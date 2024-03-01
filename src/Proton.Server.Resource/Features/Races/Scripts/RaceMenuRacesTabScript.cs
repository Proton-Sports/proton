using System.Globalization;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Resource.Features.Races.Models;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceMenuRacesTabScript : IStartup
{
    private readonly IRaceService raceService;
    private readonly IDbContextFactory dbContextFactory;

    public RaceMenuRacesTabScript(IRaceService raceService, IDbContextFactory dbContextFactory)
    {
        this.raceService = raceService;
        this.dbContextFactory = dbContextFactory;

        raceService.ParticipantJoined += HandleParticipantJoined;
        raceService.ParticipantLeft += HandleParticipantLeft;
        AltAsync.OnClient<IPlayer, Task>("race-menu-races:getRaces", HandleGetRacesAsync);
        Alt.OnClient<IPlayer, long>("race-menu-races:getDetails", HandleGetDetails);
        Alt.OnClient<IPlayer, long>("race-menu-races:join", HandleJoin);
    }

    private async Task HandleGetRacesAsync(IPlayer player)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

        var races = raceService.Races;
        var mapIds = races.Select(x => x.MapId).ToList();
        var mapDictionary = await ctx.RaceMaps
            .Where(x => mapIds.Contains(x.Id))
            .Select(x => new { x.Id, x.Name })
            .ToDictionaryAsync(x => x.Id)
            .ConfigureAwait(false);

        player.Emit("race-menu-races:getRaces", races.Select(race =>
        {
            string name = string.Empty;
            if (mapDictionary.TryGetValue(race.MapId, out var map))
            {
                name = map.Name;
            }

            return new RaceDto
            {
                Id = race.Id,
                MaxParticipants = race.MaxParticipants,
                Name = name,
                Participants = raceService.GetParticipants(race.Id).Count,
                Status = (byte)race.Status,
            };
        }).ToList());
    }

    private void HandleGetDetails(IPlayer player, long id)
    {
        var race = raceService.Races.FirstOrDefault(x => x.Id == id);
        if (race is null) return;

        player.Emit("race-menu-races:getDetails", new RaceDetailsDto
        {
            Id = race.Id,
            MapId = race.MapId,
            Description = race.Description,
            Duration = race.Duration,
            Ghosting = race.Ghosting,
            Host = race.Host.Name,
            Laps = race.Laps,
            Participants = raceService.GetParticipants(id).Select(x => new RaceParticipantDto { Id = x.Player.Id, Name = x.Player.Name }).ToList(),
            Time = race.Time.ToString("h:mm", CultureInfo.InvariantCulture),
            Type = (byte)race.Type,
            VehicleModel = race.VehicleModel.ToString(),
            Weather = race.Weather
        });
    }

    private void HandleJoin(IPlayer player, long id)
    {
        var race = raceService.Races.FirstOrDefault(x => x.Id == id);
        if (race is null) return;

        if (raceService.TryGetRaceByParticipant(player, out var oldRace))
        {
            if (id == oldRace.Id) return;
            raceService.RemoveParticipantByPlayer(player);
        }

        raceService.AddParticipant(id, new RaceParticipant { Player = player });
    }

    private void HandleParticipantJoined(Race race, IPlayer player)
    {
        Alt.EmitAllClients("race-menu-races:participantChanged", race.Id, "joined", new RaceParticipantDto { Id = player.Id, Name = player.Name });
    }

    private void HandleParticipantLeft(Race race, IPlayer player)
    {
        Alt.EmitAllClients("race-menu-races:participantChanged", race.Id, "left", new RaceParticipantDto { Id = player.Id, Name = player.Name });
    }
}
