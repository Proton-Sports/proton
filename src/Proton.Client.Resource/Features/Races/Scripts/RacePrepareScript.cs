using System.Numerics;
using AltV.Net.Client;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AsyncAwaitBestPractices;
<<<<<<< HEAD
using Proton.Client.Core.Interfaces;
using Proton.Client.Infrastructure.Constants;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.Ipls.Abstractions;
using Proton.Client.Resource.Features.UiViews.Abstractions;
=======
using Proton.Client.Infrastructure.Interfaces;
using Proton.Client.Resource.Features.Ipls.Abstractions;
>>>>>>> main
using Proton.Shared.Constants;
using Proton.Shared.Contants;
using Proton.Shared.Dtos;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RacePrepareScript(
    IUiView uiView,
    IRaceService raceService,
    IIplService iplService,
    IScriptCameraFactory scriptCameraFactory
) : HostedService
{
<<<<<<< HEAD
    private IScriptCamera? preloadCamera;

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
        Alt.OnServer<Vector3, Vector3>("race-prepare:preloadWorld", OnPreloadWorld);
        return Task.CompletedTask;
    }
=======
	private readonly IUiView uiView;
	private readonly IRaceService raceService;
	private readonly IIplService iplService;

	public RacePrepareScript(IUiView uiView, IRaceService raceService, IIplService iplService)
	{
		this.uiView = uiView;
		this.raceService = raceService;
		this.iplService = iplService;
>>>>>>> main

    private void OnPreloadWorld(Vector3 position, Vector3 rotation)
    {
        Alt.FocusData.OverrideFocusPosition(position, Vector3.Zero);
        preloadCamera = scriptCameraFactory.CreateScriptCamera(CameraHash.Scripted, true);
        preloadCamera.Position = position + new Vector3(0f, 0f, 3f);
        preloadCamera.Rotation = rotation * 180 / MathF.PI;
        preloadCamera.Render();
    }

<<<<<<< HEAD
    private async Task HandleServerMountAsync(RacePrepareDto dto)
    {
        if (preloadCamera is not null)
        {
            preloadCamera.Dispose();
            preloadCamera = null;
        }
        var task = uiView.TryMountAsync(Route.RacePrepare);
        Task? loadIplTask = default;
        if (!string.IsNullOrEmpty(dto.IplName))
        {
            loadIplTask = iplService.LoadAsync(dto.IplName);
        }

        Alt.OnTick += DisableVehicleMovement;
        raceService.IplName = dto.IplName;
        raceService.RaceType = (RaceType)dto.RaceType;
        raceService.Dimension = dto.Dimension;
        raceService.EnsureRacePointsCapacity(dto.RacePoints.Count);
        raceService.AddRacePoints(dto.RacePoints);

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
=======
	private async Task HandleServerMountAsync(RacePrepareDto dto)
	{
		var task = uiView.TryMountAsync(Route.RacePrepare);
		Task? loadIplTask = default;
		if (!string.IsNullOrEmpty(dto.IplName))
		{
			loadIplTask = iplService.LoadAsync(dto.IplName);
		}

		Alt.OnTick += DisableVehicleMovement;
		raceService.IplName = dto.IplName;
		raceService.RaceType = (RaceType)dto.RaceType;
		raceService.Dimension = dto.Dimension;
		raceService.EnsureRacePointsCapacity(dto.RacePoints.Count);
		raceService.AddRacePoints(dto.RacePoints);
		int index = 0;
		while (index + 1 < Math.Min(dto.RacePoints.Count, 2))
		{
			var nextIndex = index + 1;
			raceService.LoadRacePoint(CheckpointType.CylinderDoubleArrow, index, nextIndex < dto.RacePoints.Count ? nextIndex : null);
			++index;
		};

		if (await task.ConfigureAwait(false))
		{
			uiView.Emit("race-prepare:setData", new RacePrepareDto { EndTime = dto.EndTime });
		}
		if (loadIplTask is not null) await loadIplTask;
	}
>>>>>>> main

        if (await task.ConfigureAwait(false))
        {
            uiView.Emit("race-prepare:setData", new RacePrepareDto { EndTime = dto.EndTime });
        }
        if (loadIplTask is not null)
        {
            await loadIplTask.ConfigureAwait(false);
        }
    }

    private void HandleOnStarted(long _)
    {
        Alt.FocusData.ClearFocusOverride();
        Alt.OnTick -= DisableVehicleMovement;
    }

    private void DisableVehicleMovement()
    {
        Alt.Natives.DisableControlAction(27, 71, true);
        Alt.Natives.DisableControlAction(27, 72, true);
        Alt.Natives.DisableControlAction(27, 76, true);
    }
}
