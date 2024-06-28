using AltV.Net;
using AltV.Net.Elements.Entities;
using Proton.Shared.Interfaces;
using Proton.Shared.Dtos;
using AltV.Net.Enums;
using Proton.Server.Resource.Features.Races.Constants;
using Proton.Server.Resource.Features.Races.Models;
using System.Globalization;
using Proton.Server.Core.Interfaces;
using AltV.Net.Async;
using Microsoft.EntityFrameworkCore;
using Proton.Shared.Constants;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceHostScript : IStartup
{
	private readonly IRaceService raceService;
	private readonly IDbContextFactory dbContextFactory;
	private long counter;

    public RaceHostScript(IRaceService raceService, IDbContextFactory dbContextFactory)
	{
		this.raceService = raceService;
		this.dbContextFactory = dbContextFactory;
		Alt.OnClient<IPlayer, RaceHostSubmitDto>("race-host:submit", HandleSubmit);
		AltAsync.OnClient<IPlayer, Task>("race-host:availableMaps", HandleAvailableMapsAsync);
		AltAsync.OnClient<IPlayer, long, Task>("race-host:getMaxRacers", HandleGetMaxRacersAsync);
	}

	private void HandleSubmit(IPlayer player, RaceHostSubmitDto dto)
	{
		if (raceService.Races.Any(x => x.Host == player))
		{
			// TODO: Error handling
			return;
		}

		if (!Enum.TryParse<VehicleModel>(dto.VehicleName, true, out var model))
		{
			// TODO: Error handling
			return;
		}

		var race = new Race
		{
			Id = ++counter,
			Host = player,
			VehicleModel = model,
			MapId = dto.MapId,
			MaxParticipants = dto.Racers,
			Duration = dto.Duration,
			CountdownSeconds = dto.Countdown,
			Description = dto.Description,
			Ghosting = dto.Ghosting,
			Type = dto.Type switch
			{
				"byLaps" => RaceType.Laps,
				"pointToPoint" => RaceType.PointToPoint,
				_ => RaceType.Laps
			},
			Laps = dto.Laps,
			Time = dto.Time switch
			{
				"earlyMorning" => new TimeOnly(5, 0, 0),
				"midday" => new TimeOnly(12, 0, 0),
				"night" => new TimeOnly(19, 0, 0),
				"exactTime" => ((Func<string?, TimeOnly>)(static (string? exactTime) =>
				{
					if (TimeOnly.TryParseExact(exactTime, "h:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
					{
						return time;
					}
					return new TimeOnly(5, 0, 0);
				}))(dto.ExactTime),
				_ => new TimeOnly(5, 0, 0),
			},
			Weather = dto.Weather,
			CreatedTime = DateTimeOffset.UtcNow,
			Status = RaceStatus.Open,
		};
		raceService.AddRace(race);
		raceService.AddParticipant(race.Id, new RaceParticipant { Player = player });
		player.Emit("race-host:submit");
	}

	private async Task HandleAvailableMapsAsync(IPlayer player)
	{
		await using var ctx = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
		var dtos = await ctx.RaceMaps
			.Select(x => new RaceMapDto { Id = x.Id, Name = x.Name })
			.ToListAsync()
			.ConfigureAwait(false);
		player.Emit("race-host:availableMaps", dtos);
	}

	private async Task HandleGetMaxRacersAsync(IPlayer player, long mapId)
	{
		await using var ctx = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
		var racers = await ctx.RaceMaps.Where(x => x.Id == mapId).Select(x => x.StartPoints.Count).FirstOrDefaultAsync().ConfigureAwait(false);
		player.Emit("race-host:getMaxRacers", Math.Max(racers, 1));
	}
}
