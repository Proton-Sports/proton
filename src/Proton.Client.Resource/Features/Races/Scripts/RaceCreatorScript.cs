using AltV.Net.Client;
using AltV.Net.Client.Async;
using AltV.Net.Client.Elements.Data;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;
using Proton.Client.Core.Interfaces;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.Ipls.Abstractions;
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
    IIplService iplService
) : HostedService
{
    private bool focusing;
    private bool canSwitch = true;
    private PointType pointType = PointType.Start;
    private long id;
    private string name = string.Empty;
    private string? iplName;
    private ICheckpoint? movingRaceCheckpoint;

    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer("race:creator:stop", HandleServerStop);
        Alt.OnServer<RaceCreatorDto>("race-menu-creator:data", HandleServerData);
        Alt.OnServer<RaceMapDto, Task>("race-menu-creator:editMap", HandleServerEditMap);
        Alt.OnServer<int>("race-menu-creator:deleteMap", HandleServerDeleteMap);
        uiView.On("race-menu-creator:data", HandleData);
        uiView.On<string>("race:creator:changeMode", HandleChangeMode);
        uiView.On<string, string, Task>("race-menu-creator:createMap", HandleCreateMap);
        uiView.On<int>("race-menu-creator:deleteMap", HandleDeleteMap);
        uiView.On<long, string>("race-menu-creator:editMap", HandleEditMap);
        uiView.On("race:creator:submit", HandleSubmit);
        uiView.On("race:creator:stop", HandleStop);
        Alt.OnWindowFocusChange += HandleWindowFocusChange;
        return Task.CompletedTask;
    }

    private async Task HandleCreateMap(string mapName, string iplName)
    {
        name = mapName;
        this.iplName = iplName;
        await StartAsync().ConfigureAwait(false);
    }

    private void HandleDeleteMap(int id)
    {
        Alt.EmitServer("race-menu-creator:deleteMap", id);
    }

    private void HandleServerDeleteMap(int id)
    {
        uiView.Emit("race-menu-creator:deleteMap", id);
    }

    private void HandleEditMap(long id, string type)
    {
        pointType = type switch
        {
            "start" => PointType.Start,
            "race" => PointType.Race,
            _ => PointType.Start
        };
        Alt.EmitServer("race-menu-creator:editMap", id, type);
    }

    private void HandleSubmit()
    {
        Alt.EmitServer(
            "race:creator:submit",
            new SharedRaceCreatorData
            {
                Id = id,
                Name = name,
                IplName = iplName,
                StartPoints = raceCreator.StartPoints.ToList(),
                RacePoints = raceCreator.RacePoints.ToList()
            }
        );
    }

    private void HandleStop()
    {
        Alt.EmitServer("race:creator:stop");
    }

    private async Task HandleServerStop()
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

    private async void HandleKeyUp(Key key)
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

    private void Unfocus()
    {
        focusing = false;
        uiView.Unfocus();
        Alt.ShowCursor(false);
        Alt.GameControlsEnabled = true;
    }

    private void Focus()
    {
        focusing = true;
        uiView.Focus();
        Alt.ShowCursor(true);
        Alt.GameControlsEnabled = false;
    }

    private void HandleKeyDown(Key key)
    {
        if (key == Key.Menu)
        {
            Focus();
        }
    }

    private void HandleWindowFocusChange(bool state)
    {
        if (!state && focusing)
        {
            Unfocus();
        }
    }

    private void HandleData()
    {
        Alt.EmitServer("race-menu-creator:data");
    }

    private void HandleChangeMode(string mode)
    {
        Alt.EmitServer("race:creator:changeMode", mode);
    }

    private void HandleServerData(RaceCreatorDto dto)
    {
        uiView.Emit("race-menu-creator:data", dto);
    }

    private async Task HandleServerEditMap(RaceMapDto map)
    {
        canSwitch = false;
        id = map.Id;
        name = map.Name;
        iplName = map.IplName;
        raceCreator.ImportStartPoints(map.StartPoints);
        raceCreator.ImportRacePoints(map.RacePoints);
        await StartAsync().ConfigureAwait(false);
    }

    private async Task StartAsync()
    {
        if (uiView.IsMounted(Route.RaceMenu))
        {
            uiView.Unmount(Route.RaceMenu);
        }
        raceCreator.ClearStartPoints();
        raceCreator.ClearRacePoints();

        if (!string.IsNullOrEmpty(iplName))
        {
            await iplService.LoadAsync(iplName).ConfigureAwait(false);
        }
        uiView.Mount(Route.RaceCreator);
        Alt.OnKeyUp += HandleKeyUp;
        Alt.OnKeyDown += HandleKeyDown;
        Alt.GameControlsEnabled = true;
    }

    private enum PointType
    {
        Start,
        Race
    }
}
