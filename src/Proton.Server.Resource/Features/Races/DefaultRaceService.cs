using AltV.Net.Elements.Entities;
using AsyncAwaitBestPractices;
using Proton.Server.Resource.Features.Races.Constants;
using Proton.Server.Resource.Features.Races.Models;

namespace Proton.Server.Resource.Features.Races;

public sealed class DefaultRaceService : IRaceService
{
    private readonly List<Race> races = [];
    private readonly Dictionary<long, List<RaceParticipant>> raceParticipants = [];
    private readonly Dictionary<IPlayer, Race> playerRace = [];

    public IReadOnlyCollection<Race> Races => new List<Race>(races);

    public event Action<Race, IPlayer>? ParticipantJoined;
    public event Action<Race, IPlayer>? ParticipantLeft;
    public event Func<Race, Task>? RacePrepared;
    public event Func<Race, Task>? RaceStarted;
    public event Action<Race>? RaceCreated;

    public void AddRace(Race race)
    {
        races.Add(race);
        raceParticipants[race.Id] = [];
        if (RaceCreated is not null) RaceCreated(race);
    }

    public bool RemoveRace(Race race)
    {
        var ret = races.Remove(race);
        return ret;
    }

    public bool DestroyRace(Race race)
    {
        var ret = races.Remove(race);
        if (raceParticipants.TryGetValue(race.Id, out var participants))
        {
            var cloned = participants.ToArray();
            participants.Clear();
            foreach (var participant in cloned)
            {
                participant.Vehicle?.Destroy();
                if (ParticipantLeft is not null) ParticipantLeft(race, participant.Player);
            }
        }
        return ret;
    }

    public bool TryGetRaceByParticipant(IPlayer participant, out Race race)
    {
        return playerRace.TryGetValue(participant, out race!);
    }

    public bool AddParticipant(long raceId, RaceParticipant participant)
    {
        if (!raceParticipants.TryGetValue(raceId, out var participants)) return false;

        var race = races.Find(x => x.Id == raceId);
        if (race is null) return false;

        participants.Add(participant);
        playerRace[participant.Player] = race;
        if (ParticipantJoined is not null) ParticipantJoined(race, participant.Player);
        return true;
    }

    public bool RemoveParticipant(RaceParticipant participant)
    {
        if (!playerRace.TryGetValue(participant.Player, out var race)) return false;
        if (!raceParticipants.TryGetValue(race.Id, out var participants))
        {
            return false;
        }
        playerRace.Remove(participant.Player);
        participants.Remove(participant);
        participant.Vehicle?.Destroy();
        if (ParticipantLeft is not null) ParticipantLeft(race, participant.Player);
        return false;
    }

    public bool RemoveParticipantByPlayer(IPlayer player)
    {
        if (!playerRace.TryGetValue(player, out var race)) return false;
        if (!raceParticipants.TryGetValue(race.Id, out var participants))
        {
            return false;
        }
        playerRace.Remove(player);
        for (var i = 0; i != participants.Count; ++i)
        {
            var participant = participants[i];
            if (participant.Player != player) continue;
            participants.RemoveAt(i);
            participant.Vehicle?.Destroy();
            if (ParticipantLeft is not null) ParticipantLeft(race, participant.Player);
            return true;
        }
        return false;
    }

    public IReadOnlyCollection<RaceParticipant> GetParticipants(long raceId)
    {
        var race = races.Find(x => x.Id == raceId);
        return raceParticipants.TryGetValue(raceId, out var participants) ? participants : [];
    }

    public void Prepare(Race race)
    {
        race.Status = RaceStatus.Preparing;
        if (RacePrepared is not null)
        {
            foreach (var handler in RacePrepared.GetInvocationList().Cast<Func<Race, Task>>())
            {
                handler(race).SafeFireAndForget(Console.WriteLine);
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
                handler(race).SafeFireAndForget(Console.WriteLine);
            }
        }
    }
}
