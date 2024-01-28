using AltV.Net.Client;
using AltV.Net.Client.Async;
using AltV.Net.Client.Elements.Data;
using AltV.Net.Data;
using Proton.Client.Core.Interfaces;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Shared.Contants;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;
using Proton.Shared.Models;

namespace Proton.Client.Resource.Races.Scripts;

public sealed class RaceCreatorScript : IStartup
{
    private readonly IRaceCreator raceCreator;
    private readonly INoClip noClip;
    private readonly IGameplayCamera gameplayCamera;
    private readonly IRaycastService raycastService;
    private readonly IUiView uiView;
    private bool focusing = false;
    private bool canSwitch = true;
    private PointType pointType = PointType.Start;
    private long id;
    private string name = string.Empty;

    public RaceCreatorScript(
        IRaceCreator raceCreator,
        INoClip noClip,
        IGameplayCamera gameplayCamera,
        IRaycastService raycastService,
        IUiView uiView)
    {
        this.raceCreator = raceCreator;
        this.noClip = noClip;
        this.gameplayCamera = gameplayCamera;
        this.raycastService = raycastService;
        this.uiView = uiView;

        Alt.OnServer("race:creator:stop", HandleServerStop);
        Alt.OnServer<List<RaceMapDto>>("race:creator:map", HandleServerMap);
        Alt.OnServer<RaceMapDto>("race:creator:editMap", HandleServerEditMap);
        Alt.OnServer<int>("race:creator:deleteMap", HandleServerDeleteMap);
        uiView.On("race:creator:map", HandleMap);
        uiView.On<string>("race:creator:changeMode", HandleChangeMode);
        uiView.On<string>("race:creator:createMap", HandleCreateMap);
        uiView.On<int>("race:creator:deleteMap", HandleDeleteMap);
        uiView.On<long, string>("race:creator:editMap", HandleEditMap);
        uiView.On("race:creator:submit", HandleSubmit);
        uiView.On("race:creator:stop", HandleStop);
        Alt.OnWindowFocusChange += HandleWindowFocusChange;
    }

    private void HandleCreateMap(string mapName)
    {
        name = mapName;
        Start();
    }

    private void HandleDeleteMap(int id)
    {
        Alt.EmitServer("race:creator:deleteMap", id);
    }

    private void HandleServerDeleteMap(int id)
    {
        uiView.Emit("race:creator:deleteMap", id);
    }

    private void HandleEditMap(long id, string type)
    {
        this.id = id;
        pointType = type switch
        {
            "start" => PointType.Start,
            "race" => PointType.Race,
            _ => PointType.Start
        };
        Alt.EmitServer("race:creator:editMap", id, type);
    }

    private void HandleSubmit()
    {
        Alt.EmitServer("race:creator:submit", new SharedRaceCreatorData(id, name, raceCreator.StartPoints.ToList(), raceCreator.RacePoints.ToList()));
    }

    private void HandleStop()
    {
        Alt.EmitServer("race:creator:stop");
    }

    private void HandleServerStop()
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
    }

    private async void HandleKeyUp(Key key)
    {
        switch (key)
        {
            case Key.LButton:
                {
                    if (!noClip.IsStarted || focusing) break;
                    var position = gameplayCamera.Position;
                    var data = await raycastService.RaycastAsync(position, position + gameplayCamera.ForwardVector * 1000).ConfigureAwait(false);
                    if (data is { IsHit: true })
                    {
                        AltAsync.RunOnMainThread((state) =>
                        {
                            switch (pointType)
                            {
                                case PointType.Start:
                                    raceCreator.AddStartPoint((Position)state, new Rotation(0, 0, gameplayCamera.Rotation.Z * MathF.PI / 180));
                                    break;
                                case PointType.Race:
                                    raceCreator.AddRacePoint((Position)state, 4f);
                                    break;
                            }
                        }, (Position)data.EndPosition);
                    }
                    break;
                }
            case Key.RButton:
                {
                    if (!noClip.IsStarted || focusing) break;
                    switch (pointType)
                    {
                        case PointType.Start:
                            raceCreator.RemoveStartPoint();
                            break;
                        case PointType.Race:
                            raceCreator.RemoveRacePoint();
                            break;
                    }
                    break;
                }
            case Key.Z:
                {
                    if (noClip.IsStarted) break;
                    var player = Alt.LocalPlayer;
                    var position = Alt.LocalPlayer.Position;
                    if (!Alt.Natives.GetGroundZExcludingObjectsFor3dCoord(position.X, position.Y, position.Z, ref position.Z, true, true))
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
                    if (noClip.IsStarted) break;
                    switch (pointType)
                    {
                        case PointType.Start:
                            raceCreator.RemoveStartPoint();
                            break;
                        case PointType.Race:
                            raceCreator.RemoveRacePoint();
                            break;
                    }
                    break;
                }
            case Key.D1:
                {
                    if (!canSwitch || pointType == PointType.Start) return;
                    pointType = PointType.Start;
                    break;
                }
            case Key.D2:
                {
                    if (!canSwitch || pointType == PointType.Race) return;
                    pointType = PointType.Race;
                    break;
                }
            case Key.U:
                {
                    if (raceCreator.TryGetLastRacePointRadius(out var radius) && radius > 1f)
                    {
                        _ = raceCreator.TrySetLastRacePointRadius(Math.Max(1, radius - 0.5f));
                    }
                    break;
                }
            case Key.N:
                {
                    if (raceCreator.TryGetLastRacePointRadius(out var radius))
                    {
                        _ = raceCreator.TrySetLastRacePointRadius(radius + 0.5f);
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

    private void HandleMap()
    {
        Alt.EmitServer("race:creator:map");
    }

    private void HandleChangeMode(string mode)
    {
        Alt.EmitServer("race:creator:changeMode", mode);
    }

    private void HandleServerMap(List<RaceMapDto> maps)
    {
        uiView.Emit("race:creator:map", maps);
    }

    private void HandleServerEditMap(RaceMapDto map)
    {
        canSwitch = false;
        name = map.Name;
        Start();
        raceCreator.ImportStartPoints(map.StartPoints);
        raceCreator.ImportRacePoints(map.RacePoints);
    }

    private void Start()
    {
        raceCreator.ClearStartPoints();
        raceCreator.ClearRacePoints();
        uiView.Unmount(Route.RaceMainMenu);
        uiView.Mount(Route.RaceCreator);
        uiView.Unfocus();
        Alt.ShowCursor(false);
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
