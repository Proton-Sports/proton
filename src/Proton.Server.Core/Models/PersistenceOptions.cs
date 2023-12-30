using System.ComponentModel.DataAnnotations;

namespace Proton.Server.Core;

public sealed class PersistenceOptions
{
    public const string Section = "Persistence";

    [Required]
    public required string ConnectionString { get; set; }
}
