using Proton.Server.Core.Tables.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Core.Tables
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public ulong DiscordId { get; set; } = 0;

        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
