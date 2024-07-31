using AltV.Net.Elements.Entities;
using Proton.Server.Resource.Features.Races.Abstractions;

namespace Proton.Server.Resource.Features.Races;

public interface IRaceService
{
    event Action<Race, IPlayer>? ParticipantJoined;
    event Action<Race, IPlayer>? ParticipantLeft;
    event Func<Race, Task>? RacePrepared;
    event Func<Race, Task>? RaceStarted;
    event Action<Race>? RaceCreated;
    event Action<RaceParticipant>? ParticipantFinished;
    event Func<Race, Task>? RaceFinished;
    event Action<Race>? RaceDestroyed;
    event Action<Race, TimeSpan>? RaceCountdown;

    IReadOnlyCollection<Race> Races { get; }

    void AddRace(Race race);
    bool RemoveRace(Race race);
    bool DestroyRace(Race race);
    bool TryGetRaceByParticipant(IPlayer participant, out Race race);
    bool AddParticipant(long raceId, RaceParticipant participant);
    bool RemoveParticipant(RaceParticipant participant);
    bool RemoveParticipantByPlayer(IPlayer player);
    void Prepare(Race race);
    void Start(Race race);
    void Finish(RaceParticipant participant);
    void Finish(Race race);
    void Countdown(Race race, TimeSpan countDelay);
}
