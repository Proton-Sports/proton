using AltV.Net.Elements.Entities;

namespace Proton.Server.Resource.Features.Races.Models;

public class RacePointLog
{
	public int Lap { get; set; }
	public int Index { get; set; }
	public DateTimeOffset Time { get; set; }
}

