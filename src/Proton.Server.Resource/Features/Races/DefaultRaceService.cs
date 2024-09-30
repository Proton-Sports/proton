using AltV.Net;
using AltV.Net.Elements.Entities;
using AsyncAwaitBestPractices;
using Proton.Server.Resource.Features.Races.Abstractions;

namespace Proton.Server.Resource.Features.Races;

public sealed class DefaultRaceService : IRaceService
{
    private readonly List<Race> races = [];
    private readonly Dictionary<IPlayer, Race> playerRace = [];

    public IReadOnlyCollection<Race> Races => new List<Race>(races);

    public event Action<Race, RaceParticipant>? ParticipantJoined;
    public event Action<Race, IPlayer>? ParticipantLeft;
    public event Func<Race, Task>? RacePrepared;
    public event Func<Race, Task>? RaceStarted;
    public event Action<Race>? RaceCreated;
    public event Action<RaceParticipant>? ParticipantFinished;
    public event Action<Race>? RaceDestroyed;
    public event Func<Race, Task>? RaceFinished;
    public event Action<Race, TimeSpan>? RaceCountdown;

    public void AddRace(Race race)
    {
        races.Add(race);
        if (RaceCreated is not null)
        {
            RaceCreated(race);
        }
    }

    public bool RemoveRace(Race race)
    {
        var ret = races.Remove(race);
        return ret;
    }

    public bool DestroyRace(Race race)
    {
        var ret = races.Remove(race);
        var cloned = race.Participants.ToArray();
        race.Participants.Clear();
        foreach (var participant in cloned)
        {
            playerRace.Remove(participant.Player);
            participant.Vehicle?.Destroy();
            if (ParticipantLeft is not null)
            {
                ParticipantLeft(race, participant.Player);
            }
        }
        if (RaceDestroyed is not null)
        {
            RaceDestroyed(race);
        }
        return ret;
    }

    public bool TryGetRaceByParticipant(IPlayer participant, out Race race)
    {
        return playerRace.TryGetValue(participant, out race!);
    }

    public bool AddParticipant(long raceId, RaceParticipant participant)
    {
        var race = races.Find(x => x.Id == raceId);
        if (race is null)
        {
            return false;
        }

        race.Participants.Add(participant);
        playerRace[participant.Player] = race;
        if (ParticipantJoined is not null)
        {
            ParticipantJoined(race, participant);
        }

        return true;
    }

    public bool RemoveParticipant(RaceParticipant participant)
    {
        if (!playerRace.TryGetValue(participant.Player, out var race))
        {
            return false;
        }

        playerRace.Remove(participant.Player);
        var removed = race.Participants.Remove(participant);
        participant.Vehicle?.Destroy();
        if (ParticipantLeft is not null)
        {
            ParticipantLeft(race, participant.Player);
        }

        return removed;
    }

    public bool RemoveParticipantByPlayer(IPlayer player)
    {
        if (!playerRace.TryGetValue(player, out var race))
        {
            return false;
        }

        playerRace.Remove(player);
        for (var i = 0; i != race.Participants.Count; ++i)
        {
            var participant = race.Participants[i];
            if (participant.Player != player)
            {
                continue;
            }

            race.Participants.RemoveAt(i);
            participant.Vehicle?.Destroy();
            if (ParticipantLeft is not null)
            {
                ParticipantLeft(race, participant.Player);
            }

            return true;
        }
        return false;
    }

    public void Prepare(Race race)
    {
        race.Status = RaceStatus.Preparing;
        if (RacePrepared is not null)
        {
            foreach (var handler in RacePrepared.GetInvocationList().Cast<Func<Race, Task>>())
            {
                handler(race).SafeFireAndForget(e => Alt.LogError(e.ToString()));
            }
        }
    }

    public void Start(Race race)
    {
        race.Status = RaceStatus.Started;
        if (RaceStarted is not null)
        {
            foreach (var handler in RaceStarted.GetInvocationList().Cast<Func<Race, Task>>())
            {
                handler(race).SafeFireAndForget(e => Alt.LogError(e.ToString()));
            }
        }
    }

    public void Finish(RaceParticipant participant)
    {
        if (ParticipantFinished is not null)
        {
            ParticipantFinished(participant);
        }
    }

    public void Finish(Race race)
    {
        if (RaceFinished is not null)
        {
            RaceFinished(race).SafeFireAndForget(exception => Alt.LogError(exception.ToString()));
        }
    }

    public void Countdown(Race race, TimeSpan countDelay)
    {
        if (RaceCountdown is not null)
        {
            RaceCountdown(race, countDelay);
        }
    }
}
