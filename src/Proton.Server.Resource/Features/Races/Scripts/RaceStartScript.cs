using AltV.Net;
using AltV.Net.Elements.Entities;
using Proton.Server.Resource.Features.Races.Models;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource.Features.Races.Scripts;

public sealed class RaceStartScript : IStartup
{
	private readonly IRaceService raceService;

	public RaceStartScript(IRaceService raceService)
	{
		this.raceService = raceService;
		raceService.RaceStarted += HandleRaceStartedAsync;
		Alt.OnClient<IPlayer>("race-start:finish", HandleFinish);
	}

	private Task HandleRaceStartedAsync(Race race)
	{
		var participants = raceService.GetParticipants(race.Id);
		Alt.EmitClients(participants.Select(x => x.Player).ToArray(), "race-start:start", new RaceStartDto { Laps = race.Laps ?? 1, Ghosting = race.Ghosting });
		foreach (var participant in participants.Where(x => x.Vehicle is not null))
		{
			participant.Vehicle!.Frozen = false;
			participant.Vehicle!.EngineOn = true;
		}
		return Task.CompletedTask;
	}

	private void HandleFinish(IPlayer player)
	{
		if (!raceService.TryGetRaceByParticipant(player, out var race)) return;

		var now = DateTimeOffset.UtcNow;
		var participants = raceService.GetParticipants(race.Id);
		var finishedCount = 0;
		foreach (var participant in participants)
		{
			if (participant.Player == player)
			{
				participant.FinishTime = now.ToUnixTimeMilliseconds();
				++finishedCount;
			}
			else if (participant.FinishTime != 0) ++finishedCount;
		}

		if (finishedCount == participants.Count)
		{
			var players = participants.Select(x => x.Player).ToArray();
			raceService.DestroyRace(race);
			Alt.EmitClients(players, "race:destroy");
		}
		else if (finishedCount == 1)
		{
			Alt.EmitClients([.. participants.Select(x => x.Player)], "race-end:countdown", new RaceEndCountdownDto
			{
				EndTime = DateTimeOffset.UtcNow.AddSeconds(race.Duration).ToUnixTimeMilliseconds()
			});
		}
	}
}
