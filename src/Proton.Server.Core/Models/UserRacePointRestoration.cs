namespace Proton.Server.Core.Models;

public sealed record UserRacePointRestoration
{
    public long UserId { get; init; }
    public int Lap { get; init; }
    public int Index { get; init; }
    public DateTimeOffset Time { get; init; }
}
