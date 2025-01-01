using Proton.Server.Core.Models.Log;
using Proton.Server.Core.Models.Shop;

namespace Proton.Server.Core.Models;

public class User
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public ulong DiscordId { get; set; }
    public int Money { get; set; }
    public Character? Character { get; set; }
    public UserRole Role { get; set; }

    public List<PlayerVehicle> Vehicles { get; set; } = [];
    public ICollection<Session> Sessions { get; set; } = [];
    public List<Closet> Closets { get; set; } = [];
}
