using AltV.Net.Elements.Entities;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RacePrizePoolScript(IRaceService raceService) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        raceService.ParticipantJoined += OnParticipantJoined;
        raceService.ParticipantLeft += OnParticipantLeft;
        return Task.CompletedTask;
    }

    private void OnParticipantJoined(Race race, RaceParticipant participant)
    {
        race.PrizePool += 200;
    }

    private void OnParticipantLeft(Race race, IPlayer player)
    {
        race.PrizePool = Math.Max(race.PrizePool - 200, 0);
    }
}
