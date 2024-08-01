using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceRespawnScript(IRaceService raceService, IMapCache mapCache) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        AltAsync.OnClient<IPlayer, Task>("race-respawn:respawn", OnRespawnAsync);
        return Task.CompletedTask;
    }

    private async Task OnRespawnAsync(IPlayer player)
    {
        if (!raceService.TryGetRaceByParticipant(player, out var race))
        {
            return;
        }

        var index = race.Participants.FindIndex((x) => x.Player == player);
        if (index == -1)
        {
            return;
        }

        var map = await mapCache.GetAsync(race.MapId).ConfigureAwait(false);
        if (map is null)
        {
            return;
        }

        var entity = (IEntity)player.Vehicle ?? player;
        var participant = race.Participants[index];
        if (participant.PointLogs.Last is not null)
        {
            entity.Position = map.RacePoints[participant.PointLogs.Last.Value.Index].Position;
        }
        else
        {
            entity.Position = map.StartPoints[index].Position;
        }
    }
}
