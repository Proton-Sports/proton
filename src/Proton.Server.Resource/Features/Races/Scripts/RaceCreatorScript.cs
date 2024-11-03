using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Proton.Server.Core.Interfaces;
using Proton.Server.Core.Models;
using Proton.Server.Infrastructure.Interfaces;
using Proton.Server.Resource.Features.Ipls.Abstractions;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;
using Proton.Shared.Models;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceCreatorScript(
    INoClip noClip,
    IDbContextFactory dbContextFactory,
    IOptionsMonitor<IplOptions> iplOptions
) : HostedService
{
    private readonly Dictionary<IPlayer, WorldState> playerWorldStates = [];

    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnPlayerConnect += (player, reason) =>
        {
            var position = new Position(443.94177f, 5605.972f, -80.677635f);
            player.Spawn(position);
            player.Emit("race:creator:start");
            player.Model = (uint)PedModel.FreemodeMale01;
        };
        Alt.OnClient("race:creator:stop", HandleStop);
        Alt.OnClient<IPlayer, string>("race:creator:changeMode", HandleChangeMode);
        AltAsync.OnClient<IPlayer, long, string, Task>("race-menu-creator:editMap", HandleEditMapAsync);
        AltAsync.OnClient<IPlayer, Task>("race-menu-creator:data", HandleDataAsync);
        AltAsync.OnClient<IPlayer, SharedRaceCreatorData, Task>("race:creator:submit", HandleSubmitAsync);
        AltAsync.OnClient<IPlayer, int, Task>("race-menu-creator:deleteMap", HandleDeleteMapAsync);
        return Task.CompletedTask;
    }

    private void HandleStop(IPlayer player)
    {
        TryStopNoClip(player);
        player.Emit("race:creator:stop");
    }

    private void HandleChangeMode(IPlayer player, string mode)
    {
        switch (mode)
        {
            case "free":
                if (noClip.IsStarted(player))
                {
                    break;
                }

                player.Visible = false;
                player.Invincible = true;
                player.Frozen = true;
                player.Collision = false;
                playerWorldStates[player] = new() { Position = player.Position, Rotation = player.Rotation, };
                noClip.Start(player);
                break;
            case "normal":
                TryStopNoClip(player);
                break;
        }
    }

    private async Task HandleSubmitAsync(IPlayer player, SharedRaceCreatorData data)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        if (data.Id == 0)
        {
            ctx.Add(
                new RaceMap
                {
                    Id = data.Id,
                    Name = data.Name,
                    IplName = data.IplName,
                    StartPoints = data
                        .StartPoints.Select(
                            (point, index) =>
                                new RaceStartPoint
                                {
                                    Index = index,
                                    Position = point.Position,
                                    Rotation = point.Rotation
                                }
                        )
                        .ToArray(),
                    RacePoints = data
                        .RacePoints.Select(
                            (point, index) =>
                                new RacePoint
                                {
                                    Index = index,
                                    Position = point.Position,
                                    Radius = point.Radius
                                }
                        )
                        .ToArray()
                }
            );
            await ctx.SaveChangesAsync().ConfigureAwait(false);
        }
        else
        {
            if (data.StartPoints.Count > 0)
            {
                await ctx.RaceStartPoints.Where(x => x.MapId == data.Id).ExecuteDeleteAsync().ConfigureAwait(false);
                ctx.AddRange(
                    data.StartPoints.Select(
                            (point, index) =>
                                new RaceStartPoint
                                {
                                    MapId = data.Id,
                                    Index = index,
                                    Position = point.Position,
                                    Rotation = point.Rotation
                                }
                        )
                        .ToArray()
                );
            }
            if (data.RacePoints.Count > 0)
            {
                await ctx.RacePoints.Where(x => x.MapId == data.Id).ExecuteDeleteAsync().ConfigureAwait(false);
                ctx.AddRange(
                    data.RacePoints.Select(
                            (point, index) =>
                                new RacePoint
                                {
                                    MapId = data.Id,
                                    Index = index,
                                    Position = point.Position,
                                    Radius = point.Radius
                                }
                        )
                        .ToArray()
                );
            }
            await ctx
                .RaceMaps.Where(x => x.Id == data.Id)
                .ExecuteUpdateAsync(calls => calls.SetProperty(x => x.Name, data.Name))
                .ConfigureAwait(false);
            await ctx.SaveChangesAsync().ConfigureAwait(false);
        }
        player.Emit("race:creator:stop");
        TryStopNoClip(player);
    }

    private void TryStopNoClip(IPlayer player)
    {
        if (!noClip.IsStarted(player))
        {
            return;
        }

        player.Visible = true;
        player.Invincible = false;
        player.Frozen = false;
        player.Collision = true;
        if (playerWorldStates.Remove(player, out var state))
        {
            player.Position = state.Position;
            player.Rotation = state.Rotation;
        }
        noClip.Stop(player);
    }

    private async Task HandleDataAsync(IPlayer player)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var maps = await ctx.RaceMaps.ToArrayAsync().ConfigureAwait(false);
        player.Emit(
            "race-menu-creator:data",
            new RaceCreatorDto
            {
                Maps = maps.Select(x => new RaceCreatorMapDto { Id = x.Id, Name = x.Name }).ToList(),
                Ipls = iplOptions.CurrentValue.Entries.Select(x => x.Name).ToArray()
            }
        );
    }

    private async Task HandleEditMapAsync(IPlayer player, long id, string type)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var query = ctx.RaceMaps.Where(x => x.Id == id);
        query = type switch
        {
            "start" => query.Include(x => x.StartPoints),
            "race" => query.Include(x => x.RacePoints),
            _ => query,
        };
        var map = await query.FirstOrDefaultAsync().ConfigureAwait(false);
        if (map is null)
        {
            // TODO: Error handling
            return;
        }

        player.Emit(
            "race-menu-creator:editMap",
            new RaceMapDto
            {
                Id = map.Id,
                Name = map.Name,
                IplName = map.IplName,
                StartPoints =
                    map.StartPoints?.Select(x => new SharedRaceStartPoint(x.Position, x.Rotation)).ToList() ?? [],
                RacePoints = map.RacePoints?.Select(x => new SharedRacePoint(x.Position, x.Radius)).ToList() ?? [],
            }
        );
    }

    private async Task HandleDeleteMapAsync(IPlayer player, int id)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var count = await ctx.RaceMaps.Where(x => x.Id == id).ExecuteDeleteAsync().ConfigureAwait(false);
        if (count == 1)
        {
            player.Emit("race-menu-creator:deleteMap", id);
        }
    }

    private sealed class WorldState
    {
        public Position Position { get; set; }
        public Rotation Rotation { get; set; }
    }
}
