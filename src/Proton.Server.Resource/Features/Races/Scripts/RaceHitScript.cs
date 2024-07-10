using System.Numerics;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Constants;
using Proton.Shared.Dtos;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceHitScript(
    IRaceService raceService,
    IMapCache mapCache,
    IEnumerable<IRacePointResolver> resolvers
) : HostedService
{
    private readonly Dictionary<RaceType, IRacePointResolver> raceTypeResolvers = resolvers.ToDictionary(x =>
        x.SupportedRaceType
    );

    public override Task StartAsync(CancellationToken ct)
    {
        AltAsync.OnClient<IPlayer, int, Task>("race:hit", OnHitAsync);
        return Task.CompletedTask;
    }

    private async Task OnHitAsync(IPlayer player, int index)
    {
        if (
            !raceService.TryGetRaceByParticipant(player, out var race)
            || !raceTypeResolvers.TryGetValue(race.Type, out var resolver)
        )
        {
            return;
        }

        var participantIndex = race.Participants.FindIndex(x => x.Player == player);
        if (participantIndex == -1)
        {
            return;
        }

        var map = await mapCache.GetAsync(race.MapId).ConfigureAwait(false);
        if (map is null)
        {
            return;
        }

        var participant = race.Participants[participantIndex];
        var lap = participant.Lap;
        var now = DateTimeOffset.UtcNow;
        var output = resolver.Resolve(
            new RacePointResolverInput
            {
                Index = index,
                Lap = lap,
                TotalLaps = race.Laps ?? 1,
                TotalPoints = map.RacePoints.Count,
            }
        );
        participant.Lap = output.Lap;

        if (output.Finished)
        {
            participant.NextRacePointIndex = null;
            participant.FinishTime = now.ToUnixTimeMilliseconds();
            raceService.Finish(participant);
            player.Emit("race:finish");
        }
        else
        {
            participant.NextRacePointIndex = output.Index;
            player.Emit(
                "race:hit",
                new RaceHitDto
                {
                    Lap = lap,
                    Index = output.Index,
                    NextIndex = output.NextIndex,
                    Finished = output.Finished,
                }
            );
        }

        participant.AccumulatedDistance += Vector3.Distance(
            map.RacePoints[index].Position,
            (lap, index) switch
            {
                (0, 0) => map.StartPoints[participantIndex].Position,
                (_, 0) => map.RacePoints[^1].Position,
                _ => map.RacePoints[index - 1].Position,
            }
        );
        participant.PointLogs.AddLast(
            new RacePointLog
            {
                Lap = lap,
                Index = index,
                Time = now,
            }
        );
    }
}
