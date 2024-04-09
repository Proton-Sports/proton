using System.ComponentModel.DataAnnotations;

namespace Proton.Server.Resource.Features.Ipls.Abstractions;

public class IplOptionsEntryPosition
{
    [Required] public required float X { get; set; }
    [Required] public required float Y { get; set; }
    [Required] public required float Z { get; set; }
}
