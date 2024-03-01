using AltV.Net;
using Proton.Server.Resource.Features.Races.Constants;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceEndScript : IStartup
{
    private readonly IRaceService raceService;
    private readonly Timer timer;

    public RaceEndScript(IRaceService raceService)
    {
        this.raceService = raceService;
        timer = new Timer((state) => HandleTimerTick(), null, 1000, Timeout.Infinite);
    }

    private void HandleTimerTick()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var race in raceService.Races.Where(x => x.Status == RaceStatus.Started))
        {
            var participants = raceService.GetParticipants(race.Id);
            var earlistFinishTime = long.MaxValue;
            foreach (var participant in participants)
            {
                if (participant.FinishTime == 0 || participant.FinishTime > earlistFinishTime) continue;
                earlistFinishTime = participant.FinishTime;
            }
            if (earlistFinishTime == long.MaxValue) continue;


            if (now >= DateTimeOffset.FromUnixTimeMilliseconds(earlistFinishTime).AddSeconds(race.Duration))
            {
                var players = participants.Select(x => x.Player).ToArray();
                race.Status = RaceStatus.Ended;
                raceService.DestroyRace(race);
                Alt.EmitClients(players, "race:end");
            }
        }
        timer.Change(1000, Timeout.Infinite);
    }
}
