using System.Numerics;
using AltV.Net.Client;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AsyncAwaitBestPractices;
using Proton.Client.Core.Interfaces;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.Ipls.Abstractions;
using Proton.Client.Resource.Features.Races.Abstractions;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Dtos;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RacePrepareScript(IUiView uiView, IRaceService raceService, IIplService iplService) : HostedService
{
    IScriptCamera? preloadCamera;

    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer<RacePrepareDto>(
            "race-prepare:mount",
            (dto) =>
            {
                HandleServerMountAsync(dto).SafeFireAndForget((exception) => Alt.LogError(exception.Message));
            }
        );
        Alt.OnServer<long>("race:start", HandleOnStarted);
        Alt.OnServer<Vector3>("race-prepare:enterTransition", OnEnterTransition);
        Alt.OnServer("race-prepare:exitTransition", OnExitTransition);
        Alt.OnServer("race:finish", OnRaceFinished);
        return Task.CompletedTask;
    }

    void OnEnterTransition(Vector3 position)
    {
        raceService.Status = RaceStatus.Preparing;
        uiView.Mount(Route.RacePrepareTransition);
        Alt.OnTick += DisableVehicleActions;
        Alt.OnTick += DisableLeavingVehicle;
        Alt.Natives.DoScreenFadeOut(1000);
        Alt.SetTimeout(
            () =>
            {
                Alt.FocusData.OverrideFocusPosition(position, new Position(64, 64, 64));
            },
            500
        );
    }

    void OnExitTransition()
    {
        uiView.Unmount(Route.RacePrepareTransition);
        Alt.Natives.DoScreenFadeIn(1000);
        Alt.SetTimeout(Alt.FocusData.ClearFocusOverride, 500);
    }

    async Task HandleServerMountAsync(RacePrepareDto dto)
    {
        if (preloadCamera is not null)
        {
            preloadCamera.Dispose();
            preloadCamera = null;
        }
        Task? loadIplTask = default;
        if (!string.IsNullOrEmpty(dto.IplName))
        {
            loadIplTask = iplService.LoadAsync(dto.IplName);
        }

        raceService.IplName = dto.IplName;
        raceService.RaceType = (RaceType)dto.RaceType;
        raceService.Dimension = dto.Dimension;
        raceService.EnsureRacePointsCapacity(dto.RacePoints.Count);
        raceService.AddRacePoints(dto.RacePoints);

        if (!dto.DisableLoadingCheckpoint)
        {
            var index = 0;
            while (index + 1 < Math.Min(dto.RacePoints.Count, 2))
            {
                var nextIndex = index + 1;
                raceService.LoadRacePoint(
                    CheckpointType.CylinderDoubleArrow,
                    index,
                    nextIndex < dto.RacePoints.Count ? nextIndex : null
                );
                ++index;
            }
        }

        if (loadIplTask is not null)
        {
            await loadIplTask.ConfigureAwait(false);
        }
    }

    void HandleOnStarted(long _)
    {
        Alt.OnTick -= DisableVehicleActions;
    }

    void DisableVehicleActions()
    {
        Alt.Natives.DisableControlAction(27, 71, true);
        Alt.Natives.DisableControlAction(27, 72, true);
        Alt.Natives.DisableControlAction(27, 76, true);
    }

    void DisableLeavingVehicle()
    {
        const int INPUT_VEH_EXIT = 75;
        Alt.Natives.DisableControlAction(27, INPUT_VEH_EXIT, true);
    }

    void OnRaceFinished()
    {
        Alt.OnTick -= DisableLeavingVehicle;
    }
}
