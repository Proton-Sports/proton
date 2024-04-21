using Proton.Server.Core.Interfaces;
using Proton.Server.Core.Models.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Core.Models
{
    public class User : IAggregateRoot
    {
        public long Id { get; set; }
        public string Username { get; set; } = "";
        public ulong DiscordId { get; set; } = 0;
        public int Money { get; set; } = 0;

        public Character? Character { get; set; }
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        public ICollection<OwnedVehicle> Garage { get; set; } = new List<OwnedVehicle>();
    }
}
