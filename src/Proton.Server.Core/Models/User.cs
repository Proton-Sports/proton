using Proton.Server.Core.Models.Log;
using Proton.Server.Core.Models.Shop;

namespace Proton.Server.Core.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; } = "";
        public ulong DiscordId { get; set; } = 0;
        public int Money { get; set; } = 0;
        public Character? Character { get; set; }
        public UserRole Role { get; set; }

        public List<Garage> Garages { get; set; } = [];
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        public List<Closet> Closets { get; set; } = [];
    }
}
