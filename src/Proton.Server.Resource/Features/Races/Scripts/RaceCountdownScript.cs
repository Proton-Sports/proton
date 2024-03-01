using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Resource.Features.Races.Constants;
using Proton.Server.Resource.Features.Races.Models;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceCountdownScript : IStartup
{
    private readonly IRaceService raceService;
    private readonly IDbContextFactory dbContextFactory;
    private readonly Timer timer;

    public RaceCountdownScript(IRaceService raceService, IDbContextFactory dbContextFactory)
    {
        this.raceService = raceService;
        this.dbContextFactory = dbContextFactory;

        raceService.ParticipantJoined += HandleParticipantJoined;
        raceService.ParticipantLeft += HandleParticipantLeft;
        raceService.RacePrepared += HandleRacePrepared;
        Alt.OnPlayerDisconnect += HandlePlayerDisconnect;
        AltAsync.OnClient<IPlayer, Task>("race-countdown:getData", HandleGetDataAsync);
        timer = new Timer((_) => HandleTimerTick(), null, 1000, Timeout.Infinite);
    }

    private async Task HandleGetDataAsync(IPlayer player)
    {
        if (!raceService.TryGetRaceByParticipant(player, out var race)) return;

        await using var ctx = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var mapName = await ctx.RaceMaps
            .Where(x => x.Id == race.MapId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (mapName is null) return;

        player.Emit("race-countdown:getData", new RaceCountdownDataDto
        {
            MapName = mapName,
            EndTime = race.CreatedTime.AddSeconds(race.CountdownSeconds).ToUnixTimeMilliseconds(),
            Participants = raceService.GetParticipants(race.Id).Count,
            MaxParticipants = race.MaxParticipants
        });
    }

    private void HandleParticipantJoined(Race race, IPlayer player)
    {
        player.Emit("race-countdown:mount");

        var participants = raceService.GetParticipants(race.Id);
        Alt.EmitClients([.. participants.Where(x => x != player).Select(x => x.Player)], "race-countdown:setParticipants", participants.Count);
    }

    private void HandleParticipantLeft(Race race, IPlayer player)
    {
        player.Emit("race-countdown:unmount");

        var participants = raceService.GetParticipants(race.Id);
        Alt.EmitClients([.. participants.Where(x => x != player).Select(x => x.Player)], "race-countdown:setParticipants", participants.Count);
    }

    private void HandlePlayerDisconnect(IPlayer player, string reason)
    {
        if (raceService.TryGetRaceByParticipant(player, out var race))
        {
            foreach (var participant in raceService.GetParticipants(race.Id))
            {
                if (participant.Player == player)
                {
                    raceService.RemoveParticipant(participant);
                    break;
                }
            }
        }
    }

    private void HandleTimerTick()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var race in raceService.Races.Where(x => x.Status == RaceStatus.Open && now >= x.CreatedTime.AddSeconds(x.CountdownSeconds)))
        {
            AltAsync.Do(() =>
            {
                race.PreparationEndTime = DateTimeOffset.UtcNow.AddSeconds(3);
                raceService.Prepare(race);
            });
        }
        timer.Change(1000, Timeout.Infinite);
    }

    private Task HandleRacePrepared(Race race)
    {
        var participants = raceService.GetParticipants(race.Id);
        if (participants.Count == 0)
        {
            raceService.RemoveRace(race);
            return Task.CompletedTask;
        }

        Alt.EmitClients([.. participants.Select(x => x.Player)], "race-countdown:unmount");
        return Task.CompletedTask;
    }
}
