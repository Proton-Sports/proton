using AltV.Net;
using AltV.Net.Elements.Entities;
using Proton.Shared.Interfaces;
using Proton.Shared.Dtos;
using System.Collections.Concurrent;
using Proton.Server.Infrastructure.Models;
using AltV.Net.Enums;
using Proton.Server.Infrastructure.Constants;
using System.Globalization;
using Proton.Server.Core.Interfaces;
using AltV.Net.Async;
using Microsoft.EntityFrameworkCore;

namespace Proton.Server.Resource.Races.Scripts;

public sealed class RaceHostScript : IStartup
{
	private readonly IDbContextFactory dbContextFactory;
	private readonly ConcurrentDictionary<IPlayer, Race> hostRaceDictionary = new();

	public RaceHostScript(IDbContextFactory dbContextFactory)
	{
		this.dbContextFactory = dbContextFactory;
		Alt.OnClient<IPlayer, RaceHostSubmitDto>("race-host:submit", HandleSubmit);
		AltAsync.OnClient<IPlayer, Task>("race-host:availableMaps", HandleAvailableMapsAsync);
		AltAsync.OnClient<IPlayer, long, Task>("race-host:getMaxRacers", HandleGetMaxRacersAsync);
	}

	private void HandleSubmit(IPlayer player, RaceHostSubmitDto dto)
	{
		if (hostRaceDictionary.ContainsKey(player))
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
			Host = player,
			VehicleModel = model,
			MapId = dto.MapId,
			Racers = dto.Racers,
			Duration = dto.Duration,
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
			Weather = dto.Weather
		};
		hostRaceDictionary[player] = race;
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
		player.Emit("race-host:getMaxRacers", Math.Min(racers, 1));
	}
}
