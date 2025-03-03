using AltV.Net.Client;
using AltV.Net.Client.Async;
using AltV.Net.Client.Elements.Data;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;
using Proton.Client.Core.Interfaces;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.Ipls.Abstractions;
using Proton.Client.Resource.Features.Races.Abstractions;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Dtos;
using Proton.Shared.Models;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceCreatorScript(
    IRaceCreator raceCreator,
    INoClip noClip,
    IRaycastService raycastService,
    IUiView uiView,
    IIplService iplService,
    IRaceService raceService
) : HostedService
{
    bool focusing;
    bool canSwitch = true;
    PointType pointType = PointType.Start;
    long id;
    string name = string.Empty;
    string? iplName;
    ICheckpoint? movingRaceCheckpoint;

    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer("race:creator:stop", HandleServerStop);
        Alt.OnServer<RaceCreatorDto>("race-menu-creator:data", HandleServerData);
        Alt.OnServer<RaceMapDto, Task>("race-menu-creator:editMap", HandleServerEditMapAsync);
        Alt.OnServer<int>("race-menu-creator:deleteMap", HandleServerDeleteMap);
        Alt.OnServer<RaceCreatorCreateMapDto>("race-menu-creator:createMap", OnServerCreateMap);
        uiView.On("race-menu-creator:data", HandleData);
        uiView.On<string>("race:creator:changeMode", HandleChangeMode);
        uiView.On<string, string>("race-menu-creator:createMap", HandleCreateMap);
        uiView.On<int>("race-menu-creator:deleteMap", HandleDeleteMap);
        uiView.On<long, string>("race-menu-creator:editMap", HandleEditMap);
        uiView.On("race:creator:submit", HandleSubmit);
        uiView.On("race:creator:stop", HandleStop);
        return Task.CompletedTask;
    }

    void HandleCreateMap(string mapName, string iplName)
    {
        if (raceService.Status != RaceStatus.None)
        {
            return;
        }
        Alt.EmitServer("race-menu-creator:createMap", mapName, iplName);
    }

    void OnServerCreateMap(RaceCreatorCreateMapDto dto)
    {
        if (uiView.IsMounted(Route.RaceMenu))
        {
            uiView.Unmount(Route.RaceMenu);
        }
        name = dto.MapName;
        iplName = dto.IplName;
        raceCreator.ClearStartPoints();
        raceCreator.ClearRacePoints();
        uiView.Mount(Route.RaceCreator);
        Alt.OnKeyUp += HandleKeyUp;
        Alt.OnKeyDown += HandleKeyDown;
        Alt.GameControlsEnabled = true;
    }

    void HandleDeleteMap(int id)
    {
        Alt.EmitServer("race-menu-creator:deleteMap", id);
    }

    void HandleServerDeleteMap(int id)
    {
        uiView.Emit("race-menu-creator:deleteMap", id);
    }

    void HandleEditMap(long id, string type)
    {
        pointType = type switch
        {
            "start" => PointType.Start,
            "race" => PointType.Race,
            _ => PointType.Start,
        };
        Alt.EmitServer("race-menu-creator:editMap", id, type);
    }

    void HandleSubmit()
    {
        Alt.EmitServer(
            "race:creator:submit",
            new SharedRaceCreatorData
            {
                Id = id,
                Name = name,
                IplName = iplName,
                StartPoints = raceCreator.StartPoints.ToList(),
                RacePoints = raceCreator.RacePoints.ToList(),
            }
        );
    }

    void HandleStop()
    {
        Alt.EmitServer("race:creator:stop");
    }

    async Task HandleServerStop()
    {
        if (focusing)
        {
            Unfocus();
        }
        canSwitch = true;
        id = default;
        name = string.Empty;
        uiView.Unmount(Route.RaceCreator);
        raceCreator.ClearStartPoints();
        raceCreator.ClearRacePoints();
        pointType = PointType.Start;
        Alt.OnKeyUp -= HandleKeyUp;
        Alt.OnKeyDown -= HandleKeyDown;
        if (!string.IsNullOrEmpty(iplName))
        {
            await iplService.UnloadAsync(iplName).ConfigureAwait(false);
            iplName = null;
        }
    }

    async void HandleKeyUp(Key key)
    {
        switch (key)
        {
            case Key.LButton:
            {
                if (!noClip.IsStarted || focusing)
                {
                    break;
                }

                var camera = noClip.Camera!;
                var position = camera.Position;
                var data = await raycastService
                    .RaycastAsync(position, position + camera.ForwardVector * 1000)
                    .ConfigureAwait(false);
                if (data is { IsHit: true })
                {
                    AltAsync.RunOnMainThread(
                        (state) =>
                        {
                            switch (pointType)
                            {
                                case PointType.Start:
                                    raceCreator.AddStartPoint(
                                        (Position)state,
                                        new Rotation(0, 0, camera.Rotation.Z * MathF.PI / 180)
                                    );
                                    break;
                                case PointType.Race:
                                    raceCreator.AddRacePoint((Position)state, 4f);
                                    break;
                            }
                        },
                        (Position)data.EndPosition
                    );
                }
                break;
            }
            case Key.RButton:
            {
                if (
                    !noClip.IsStarted
                    || focusing
                    || !noClip.TryGetRaycastData(out var data)
                    || data is not { IsHit: true }
                )
                {
                    break;
                }

                switch (pointType)
                {
                    case PointType.Start:
                        raceCreator.TryRemoveStartPoint(data.EndPosition, out var _);
                        break;
                    case PointType.Race:
                        if (
                            raceCreator.TryRemoveRacePoint(data.EndPosition, out var removed)
                            && removed.Checkpoint == movingRaceCheckpoint
                        )
                        {
                            movingRaceCheckpoint = null;
                        }
                        break;
                }
                break;
            }
            case Key.Z:
            {
                if (noClip.IsStarted)
                {
                    break;
                }

                var player = Alt.LocalPlayer;
                var position = Alt.LocalPlayer.Position;
                if (
                    !Alt.Natives.GetGroundZExcludingObjectsFor3dCoord(
                        position.X,
                        position.Y,
                        position.Z,
                        ref position.Z,
                        true,
                        true
                    )
                )
                {
                    return;
                }
                switch (pointType)
                {
                    case PointType.Start:
                        raceCreator.AddStartPoint(position, player.Rotation);
                        break;
                    case PointType.Race:
                        raceCreator.AddRacePoint(position, 4f);
                        break;
                }
                break;
            }
            case Key.X:
            {
                if (noClip.IsStarted)
                {
                    break;
                }

                switch (pointType)
                {
                    case PointType.Start:
                        raceCreator.TryRemoveStartPoint(Alt.LocalPlayer.Position, out var _);
                        break;
                    case PointType.Race:
                        if (
                            raceCreator.TryRemoveRacePoint(Alt.LocalPlayer.Position, out var removed)
                            && removed.Checkpoint == movingRaceCheckpoint
                        )
                        {
                            movingRaceCheckpoint = null;
                        }
                        break;
                }
                break;
            }
            case Key.D1:
            {
                if (!canSwitch || pointType == PointType.Start)
                {
                    return;
                }

                pointType = PointType.Start;
                break;
            }
            case Key.D2:
            {
                if (!canSwitch || pointType == PointType.Race)
                {
                    return;
                }

                pointType = PointType.Race;
                break;
            }
            case Key.U:
            {
                Position position = Alt.LocalPlayer.Position;
                if (!noClip.IsStarted)
                {
                    position = Alt.LocalPlayer.Position;
                    if (
                        !Alt.Natives.GetGroundZExcludingObjectsFor3dCoord(
                            position.X,
                            position.Y,
                            position.Z,
                            ref position.Z,
                            true,
                            true
                        )
                    )
                    {
                        return;
                    }
                }
                else if (noClip.TryGetRaycastData(out var data) && data is { IsHit: true })
                {
                    position = data.EndPosition;
                }
                else
                {
                    break;
                }

                if (raceCreator.TryGetClosestRaceCheckpointTo(position, out var checkpoint))
                {
                    checkpoint.Radius += 0.5f;
                }
                break;
            }
            case Key.N:
            {
                Position position = Alt.LocalPlayer.Position;
                if (!noClip.IsStarted)
                {
                    position = Alt.LocalPlayer.Position;
                    if (
                        !Alt.Natives.GetGroundZExcludingObjectsFor3dCoord(
                            position.X,
                            position.Y,
                            position.Z,
                            ref position.Z,
                            true,
                            true
                        )
                    )
                    {
                        return;
                    }
                }
                else if (noClip.TryGetRaycastData(out var data) && data is { IsHit: true })
                {
                    position = data.EndPosition;
                }
                else
                {
                    break;
                }

                if (raceCreator.TryGetClosestRaceCheckpointTo(position, out var checkpoint))
                {
                    checkpoint.Radius = Math.Max(1, checkpoint.Radius - 0.5f);
                }
                break;
            }
            case Key.M:
            {
                var position = Alt.LocalPlayer.Position;
                if (!noClip.IsStarted)
                {
                    position = Alt.LocalPlayer.Position;
                    if (
                        !Alt.Natives.GetGroundZExcludingObjectsFor3dCoord(
                            position.X,
                            position.Y,
                            position.Z,
                            ref position.Z,
                            true,
                            true
                        )
                    )
                    {
                        return;
                    }
                }
                else if (noClip.TryGetRaycastData(out var data) && data is { IsHit: true })
                {
                    position = data.EndPosition;
                }
                else
                {
                    break;
                }

                if (raceCreator.TryGetClosestRaceCheckpointTo(position, out var checkpoint))
                {
                    if (movingRaceCheckpoint is not null)
                    {
                        movingRaceCheckpoint.Color = new Rgba(255, 255, 255, 255);
                    }
                    movingRaceCheckpoint = checkpoint;
                    movingRaceCheckpoint.Color = new Rgba(0, 255, 0, 255);
                }
                else if (movingRaceCheckpoint is not null)
                {
                    raceCreator.UpdateRacePointPosition(movingRaceCheckpoint, position);
                    movingRaceCheckpoint.Color = new Rgba(255, 255, 255, 255);
                    movingRaceCheckpoint = default;
                }
                break;
            }
            case Key.Menu:
            {
                if (focusing)
                {
                    Unfocus();
                }
                break;
            }
        }
    }

    void Unfocus()
    {
        focusing = false;
        uiView.Unfocus();
        Alt.ShowCursor(false);
        Alt.GameControlsEnabled = true;
    }

    void Focus()
    {
        focusing = true;
        uiView.Focus();
        Alt.ShowCursor(true);
        Alt.GameControlsEnabled = false;
    }

    void HandleKeyDown(Key key)
    {
        if (key == Key.Menu)
        {
            Focus();
        }
    }

    void HandleData()
    {
        Alt.EmitServer("race-menu-creator:data");
    }

    void HandleChangeMode(string mode)
    {
        Alt.EmitServer("race:creator:changeMode", mode);
    }

    void HandleServerData(RaceCreatorDto dto)
    {
        uiView.Emit("race-menu-creator:data", dto);
    }

    async Task HandleServerEditMapAsync(RaceMapDto map)
    {
        canSwitch = false;
        id = map.Id;
        name = map.Name;
        iplName = map.IplName;

        if (!string.IsNullOrEmpty(iplName))
        {
            await iplService.LoadAsync(iplName).ConfigureAwait(false);
        }

        raceCreator.ImportStartPoints(map.StartPoints);
        raceCreator.ImportRacePoints(map.RacePoints);
        if (uiView.IsMounted(Route.RaceMenu))
        {
            uiView.Unmount(Route.RaceMenu);
        }
        uiView.Mount(Route.RaceCreator);
        Alt.OnKeyUp += HandleKeyUp;
        Alt.OnKeyDown += HandleKeyDown;
        Alt.GameControlsEnabled = true;
    }

    enum PointType
    {
        Start,
        Race,
    }
}
