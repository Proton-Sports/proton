using AltV.Net.Elements.Entities;

namespace Proton.Server.Resource.Features.Races.Models;

public class RaceParticipant
{
	public required IPlayer Player { get; set; }
	public IVehicle? Vehicle { get; set; }
	public long FinishTime { get; set; }
}

