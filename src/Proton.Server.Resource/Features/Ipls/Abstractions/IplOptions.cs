using System.ComponentModel.DataAnnotations;

namespace Proton.Server.Resource.Features.Ipls.Abstractions;

public sealed class IplOptions
{
    public const string Section = "Ipl";

    [Required] public required IplOptionsEntry[] Entries { get; set; }
}
