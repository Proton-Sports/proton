using Proton.Server.Core.Interfaces;

namespace Proton.Server.Core.Models;

public sealed record BanRecord : IAggregateRoot
{
    public required BanKind Kind { get; set; }
    public required string Value { get; set; }
}
