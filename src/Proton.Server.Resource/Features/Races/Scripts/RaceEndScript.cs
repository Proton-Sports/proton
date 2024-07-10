using AltV.Net;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceEndScript(IRaceService raceService) : HostedService
{
    private Timer? timer;

    public override Task StartAsync(CancellationToken ct)
    {
        timer = new Timer((state) => HandleTimerTick(), null, 1000, 1000);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        if (timer is not null)
        {
            await timer.DisposeAsync().ConfigureAwait(false);
        }
    }

    private void HandleTimerTick()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var race in raceService.Races.Where(x => x.Status == RaceStatus.Started))
        {
            var participants = race.Participants;
            var earlistFinishTime = long.MaxValue;
            foreach (var participant in participants)
            {
                if (participant.FinishTime == 0 || participant.FinishTime > earlistFinishTime)
                {
                    continue;
                }

                earlistFinishTime = participant.FinishTime;
            }
            if (earlistFinishTime == long.MaxValue)
            {
                continue;
            }

            if (now >= DateTimeOffset.FromUnixTimeMilliseconds(earlistFinishTime).AddSeconds(race.Duration))
            {
                Alt.EmitClients([.. participants.Select(x => x.Player)], "race:destroy");
                race.Status = RaceStatus.Ended;
                raceService.DestroyRace(race);
            }
        }
    }
}
