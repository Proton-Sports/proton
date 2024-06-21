using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AsyncAwaitBestPractices;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.Features.Races.Models;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceCountdownScript(IRaceService raceService, IMapCache mapCache) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        raceService.ParticipantJoined += HandleParticipantJoined;
        raceService.ParticipantLeft += HandleParticipantLeft;
        raceService.RacePrepared += HandleRacePrepared;
        Alt.OnPlayerDisconnect += HandlePlayerDisconnect;
        AltAsync.OnClient<IPlayer, Task>("race-countdown:getData", HandleGetDataAsync);
        return Task.CompletedTask;
    }

    private async Task HandleGetDataAsync(IPlayer player)
    {
        if (!raceService.TryGetRaceByParticipant(player, out var race))
        {
            return;
        }

        var map = await mapCache.GetAsync(race.MapId).ConfigureAwait(false);
        if (map is null)
        {
            return;
        }

        player.Emit(
            "race-countdown:getData",
            new RaceCountdownDataDto
            {
                MapName = map.Name,
                EndTime = race.CreatedTime.AddSeconds(race.CountdownSeconds).ToUnixTimeMilliseconds(),
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
