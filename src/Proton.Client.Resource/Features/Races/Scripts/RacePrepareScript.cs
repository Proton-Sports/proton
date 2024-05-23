using AltV.Net.Client;
using AltV.Net.Elements.Entities;
using AsyncAwaitBestPractices;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Contants;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RacePrepareScript : IStartup
{
	private readonly IUiView uiView;
	private readonly IRaceService raceService;

	public RacePrepareScript(IUiView uiView, IRaceService raceService)
	{
		this.uiView = uiView;
		this.raceService = raceService;

		Alt.OnServer<RacePrepareDto>("race-prepare:mount", (dto) =>
		{
			HandleServerMountAsync(dto)
				.SafeFireAndForget((exception) => Alt.LogError(exception.Message));
		});
		Alt.OnServer<long>("race:start", HandleOnStarted);
	}

	private async Task HandleServerMountAsync(RacePrepareDto dto)
	{
		var task = uiView.TryMountAsync(Route.RacePrepare);

		Alt.OnTick += DisableVehicleMovement;
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
	}

	private void HandleOnStarted(long _)
	{
		Alt.OnTick -= DisableVehicleMovement;
	}

	private void DisableVehicleMovement()
	{
		Alt.Natives.DisableControlAction(27, 71, true);
		Alt.Natives.DisableControlAction(27, 72, true);
		Alt.Natives.DisableControlAction(27, 76, true);
	}
}
