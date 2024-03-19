using AltV.Net.Client;
using AltV.Net.Elements.Entities;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Client.Resource.Features.Races.Models;
using Proton.Shared.Contants;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceStartScript : IStartup
{
    private readonly IRaceService raceService;
    private readonly IUiView uiView;

    public RaceStartScript(IRaceService raceService, IUiView uiView)
    {
        this.raceService = raceService;
        this.uiView = uiView;
        Alt.OnServer<RaceStartDto>("race-start:start", HandleServerStart);
        Alt.OnServer("race:destroy", RemoveRacePointHit);
        Alt.OnServer("race:leave", RemoveRacePointHit);
    }

    private void HandleServerStart(RaceStartDto dto)
    {
        Alt.GameControlsEnabled = true;
        if (dto.Ghosting)
        {
            Alt.Natives.SetLocalPlayerAsGhost(true, true);
            foreach (var vehicle in Alt.GetAllVehicles())
            {
                if (vehicle != Alt.LocalPlayer.Vehicle && vehicle.Spawned)
                {
                    Alt.Natives.SetEntityGhostedForGhostPlayers(vehicle, true);
                }
            }
        }
        raceService.Laps = dto.Laps;
        raceService.CurrentLap = 0;
        raceService.Start();
        raceService.RacePointHit += HandleRacePointHit;
        uiView.Unmount(Route.RacePrepare);
    }

    private void HandleRacePointHit(object state)
    {
        if (!raceService.TryGetPointResolver(out var resolver)) return;

        var index = (int)state;
        raceService.UnloadRacePoint(index);
        var output = resolver.Resolve(new RacePointResolverInput
        {
            Index = index,
            Lap = raceService.CurrentLap,
            TotalLaps = raceService.Laps,
            TotalPoints = raceService.RacePoints.Count
        });
        raceService.CurrentLap = output.Lap;

        if (output.Finished)
        {
            Alt.EmitServer("race-start:finish");
            raceService.Stop();
            return;
        }

        raceService.LoadRacePoint(
            output.NextIndex is null ? CheckpointType.CylinderCheckerboard : CheckpointType.CylinderDoubleArrow,
            output.Index,
            output.NextIndex
        );
    }

    private void RemoveRacePointHit()
    {
        raceService.RacePointHit -= HandleRacePointHit;
    }
}
