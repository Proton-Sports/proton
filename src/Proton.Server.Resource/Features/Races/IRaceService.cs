using AltV.Net.Elements.Entities;
using Proton.Server.Resource.Features.Races.Models;

namespace Proton.Server.Resource.Features.Races;

public interface IRaceService
{
    event Action<Race, IPlayer>? ParticipantJoined;
    event Action<Race, IPlayer>? ParticipantLeft;
    event Func<Race, Task>? RacePrepared;
    event Func<Race, Task>? RaceStarted;
    event Action<Race>? RaceCreated;

    IReadOnlyCollection<Race> Races { get; }

    void AddRace(Race race);
    bool RemoveRace(Race race);
    bool DestroyRace(Race race);
    bool TryGetRaceByParticipant(IPlayer participant, out Race race);
    bool AddParticipant(long raceId, RaceParticipant participant);
    bool RemoveParticipant(RaceParticipant participant);
    bool RemoveParticipantByPlayer(IPlayer player);
    IReadOnlyCollection<RaceParticipant> GetParticipants(long raceId);
    void Prepare(Race race);
    void Start(Race race);
}
