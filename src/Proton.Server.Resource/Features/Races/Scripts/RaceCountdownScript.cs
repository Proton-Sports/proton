using System.Collections.Concurrent;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AsyncAwaitBestPractices;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceCountdownScript(IRaceService raceService, IMapCache mapCache) : HostedService
{
    private readonly ConcurrentDictionary<Race, Timer> countdownTimers = [];

    public override Task StartAsync(CancellationToken ct)
    {
        raceService.ParticipantJoined += (race, participant) =>
            OnParticipantJoinedAsync(race, participant).SafeFireAndForget(e => Alt.LogError(e.ToString()));
        raceService.ParticipantLeft += HandleParticipantLeft;
        raceService.RacePrepared += HandleRacePrepared;
        AltAsync.OnClient<IPlayer, bool, Task>("race-countdown.ready.change", OnPlayerReadyChange);
        raceService.RaceDestroyed += (race) =>
            OnRaceDestroyedAsync(race).SafeFireAndForget(exception => Alt.LogError(exception.ToString()));
        Alt.OnClient<IPlayer, string>("race-countdown.vehicle.change", OnPlayerVehicleChange);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        foreach (var (_, timer) in countdownTimers)
        {
            await timer.DisposeAsync().ConfigureAwait(false);
        }
        countdownTimers.Clear();
    }

    private async Task OnParticipantJoinedAsync(Race race, RaceParticipant participant)
    {
        var player = participant.Player;
        if (race.Status != RaceStatus.Open)
        {
            return;
        }

        var map = await mapCache.GetAsync(race.MapId).ConfigureAwait(false);
        if (map is null)
        {
            return;
        }

        player.Emit(
            "race-countdown.mount",
            new RaceCountdownDto
            {
                Id = player.Id,
                MapName = map.Name,
                DurationSeconds = race.CountdownSeconds,
                Vehicles = race.VehicleModels.Select(a => a.ToString()).ToArray(),
                Participants = race
                    .Participants.Select(a => new RaceCountdownParticipantDto
                    {
                        Id = a.Player.Id,
                        Name = a.Player.Name,
                        IsHost = race.Host == a.Player,
                        IsReady = a.Ready,
                        VehicleModel = a.VehicleModel.ToString()
                    })
                    .ToArray(),
                MaxParticipants = race.MaxParticipants,
            }
        );

        Alt.EmitClients(
            race.Participants.Where(a => a.Player != player).Select(a => a.Player).ToArray(),
            "race-countdown.participants.add",
            new RaceCountdownParticipantDto
            {
                Id = player.Id,
                Name = player.Name,
                IsHost = false,
                IsReady = false,
                VehicleModel = participant.VehicleModel.ToString(),
            }
        );
    }

    private void HandleParticipantLeft(Race race, IPlayer player)
    {
        player.Emit("race-countdown.unmount");

        var participants = race.Participants;
        Alt.EmitClients(
            [.. participants.Where(x => x != player).Select(x => x.Player)],
            "race-countdown.participants.remove",
            player.Id
        );
    }

    private Task HandleRacePrepared(Race race)
    {
        var participants = race.Participants;
        if (participants.Count == 0)
        {
            raceService.RemoveRace(race);
            return Task.CompletedTask;
        }

        Alt.EmitClients([.. participants.Select(x => x.Player)], "race-countdown.unmount");
        return Task.CompletedTask;
    }

    private async Task OnPlayerReadyChange(IPlayer player, bool ready)
    {
        if (!raceService.TryGetRaceByParticipant(player, out var race))
        {
            return;
        }

        var participant = race.Participants.Find(a => a.Player == player);
        if (participant is null)
        {
            return;
        }

        Alt.EmitClients(
            race.Participants.Where(a => a.Player != player).Select(a => a.Player).ToArray(),
            "race-countdown.ready.change",
            player.Id,
            ready
        );

        participant.Ready = ready;
        if (ready != race.LobbyCountingDown)
        {
            var readyCount = race.Participants.Count(a => a.Ready);
            var thresholdCount = race.Participants.Count * 0.75f;
            if (ready && readyCount >= thresholdCount)
            {
                race.LobbyCountingDown = true;
                Alt.EmitClients(
                    race.Participants.Select(a => a.Player).ToArray(),
                    "race-countdown.countdown.set",
                    race.CountdownSeconds
                );
                countdownTimers[race] = new Timer(
                    (state) =>
                        OnCountdownFinishedAsync((Race)state!).SafeFireAndForget(e => Alt.LogError(e.ToString())),
                    race,
                    race.CountdownSeconds * 1000,
                    Timeout.Infinite
                );
            }
            else if (!ready && readyCount < thresholdCount)
            {
                if (countdownTimers.Remove(race, out var timer))
                {
                    await timer.DisposeAsync().ConfigureAwait(false);
                }
                race.LobbyCountingDown = false;
                Alt.EmitClients(
                    race.Participants.Select(a => a.Player).ToArray(),
                    "race-countdown.countdown.set",
                    (long)0
                );
            }
        }
    }

    private async Task OnCountdownFinishedAsync(Race race)
    {
        if (countdownTimers.Remove(race, out var timer))
        {
            foreach (var participant in race.Participants.Where(a => !a.Ready).ToList())
            {
                raceService.RemoveParticipant(participant);
            }

            await AltAsync
                .Do(() =>
                {
                    raceService.Prepare(race);
                })
                .ConfigureAwait(false);
            await timer.DisposeAsync().ConfigureAwait(false);
        }
    }

    private async Task OnRaceDestroyedAsync(Race race)
    {
        if (countdownTimers.Remove(race, out var timer))
        {
            await timer.DisposeAsync().ConfigureAwait(false);
        }
    }

    private void OnPlayerVehicleChange(IPlayer player, string name)
    {
        if (!raceService.TryGetRaceByParticipant(player, out var race))
        {
            return;
        }

        var participant = race.Participants.Find(a => a.Player == player);
        if (participant is null)
        {
            return;
        }

        if (!Enum.TryParse<VehicleModel>(name, out var model) || !race.VehicleModels.Any(a => a == model))
        {
            player.Emit("race-countdown.vehicle.change", player.Id, participant.VehicleModel.ToString());
            return;
        }

        participant.VehicleModel = model;
        Alt.EmitClients(
            race.Participants.Where(a => a.Player != player).Select(a => a.Player).ToArray(),
            "race-countdown.vehicle.change",
            player.Id,
            model.ToString()
        );
    }
}
