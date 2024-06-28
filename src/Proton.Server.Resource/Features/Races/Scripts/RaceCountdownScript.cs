using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Resource.Features.Races.Constants;
using Proton.Server.Resource.Features.Races.Models;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceCountdownScript(
    IRaceService raceService,
    IDbContextFactory dbContextFactory
) : HostedService
{
    private Timer? timer;

    public override Task StartAsync(CancellationToken ct)
    {
        raceService.ParticipantJoined += HandleParticipantJoined;
        raceService.ParticipantLeft += HandleParticipantLeft;
        raceService.RacePrepared += HandleRacePrepared;
        Alt.OnPlayerDisconnect += HandlePlayerDisconnect;
        AltAsync.OnClient<IPlayer, Task>("race-countdown:getData", HandleGetDataAsync);
        timer = new Timer((_) => HandleTimerTick(), null, 1000, 1000);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        if (timer is not null)
        {
            await timer.DisposeAsync().ConfigureAwait(false);
        }
    }

    private async Task HandleGetDataAsync(IPlayer player)
    {
        if (!raceService.TryGetRaceByParticipant(player, out var race))
            return;

        await using var ctx = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var mapName = await ctx
            .RaceMaps.Where(x => x.Id == race.MapId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (mapName is null)
            return;

        player.Emit(
            "race-countdown:getData",
            new RaceCountdownDataDto
            {
                MapName = mapName,
                EndTime = race
                    .CreatedTime.AddSeconds(race.CountdownSeconds)
                    .ToUnixTimeMilliseconds(),
                Participants = race.Participants.Count,
                MaxParticipants = race.MaxParticipants
            }
        );
    }

    private void HandleParticipantJoined(Race race, IPlayer player)
    {
        player.Emit("race-countdown:mount");

        var participants = race.Participants;
        Alt.EmitClients(
            [.. race.Participants.Where(x => x != player).Select(x => x.Player)],
            "race-countdown:setParticipants",
            participants.Count
        );
    }

    private void HandleParticipantLeft(Race race, IPlayer player)
    {
        player.Emit("race-countdown:unmount");

        var participants = race.Participants;
        Alt.EmitClients(
            [.. participants.Where(x => x != player).Select(x => x.Player)],
            "race-countdown:setParticipants",
            participants.Count
        );
    }

    private void HandlePlayerDisconnect(IPlayer player, string reason)
    {
        if (raceService.TryGetRaceByParticipant(player, out var race))
        {
            foreach (var participant in race.Participants)
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
        foreach (
            var race in raceService.Races.Where(x =>
                x.Status == RaceStatus.Open && now >= x.CreatedTime.AddSeconds(x.CountdownSeconds)
            )
        )
        {
            AltAsync.Do(() =>
            {
                race.PreparationEndTime = DateTimeOffset.UtcNow.AddSeconds(3);
                raceService.Prepare(race);
            });
        }
    }

    private Task HandleRacePrepared(Race race)
    {
        var participants = race.Participants;
        if (participants.Count == 0)
        {
            raceService.RemoveRace(race);
            return Task.CompletedTask;
        }

        Alt.EmitClients([.. participants.Select(x => x.Player)], "race-countdown:unmount");
        return Task.CompletedTask;
    }
}
