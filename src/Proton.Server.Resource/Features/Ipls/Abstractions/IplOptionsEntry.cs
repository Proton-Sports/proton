using System.ComponentModel.DataAnnotations;

namespace Proton.Server.Resource.Features.Ipls.Abstractions;

public sealed class IplOptionsEntry
{
    [Required] public required string Name { get; set; }
    [Required] public required IplOptionsEntryPosition Position { get; set; }
}
