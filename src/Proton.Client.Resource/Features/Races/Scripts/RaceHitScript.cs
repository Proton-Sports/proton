using AltV.Net.Client;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Elements.Entities;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceHitScript : IStartup
{
    private readonly IRaceService raceService;

    public RaceHitScript(IRaceService raceService)
    {
        this.raceService = raceService;
        Alt.OnServer<RaceStartDto>("race-start:start", HandleServerStart);
        Alt.OnServer<RaceHitDto>("race:hit", OnServerHit);
        Alt.OnServer("race:destroy", RemoveRacePointHit);
        Alt.OnServer("race:leave", RemoveRacePointHit);
        Alt.OnServer("race:finish", RemoveRacePointHit);
    }

    private void HandleServerStart(RaceStartDto dto)
    {
        raceService.RacePointHit += HandleRacePointHit;
    }

    private void HandleRacePointHit(object state)
    {
        Alt.EmitServer("race:hit", (int)state);
    }

    private void OnServerHit(RaceHitDto dto)
    {
        if (dto.Finished)
        {
            raceService.Stop();
            return;
        }

        raceService.LoadRacePoint(
            dto.NextIndex is null ? CheckpointType.CylinderCheckerboard : CheckpointType.CylinderDoubleArrow,
            dto.Index,
            dto.NextIndex
        );
    }

    private void RemoveRacePointHit()
    {
        raceService.UnloadRacePoint();
        raceService.RacePointHit -= HandleRacePointHit;
    }
}
