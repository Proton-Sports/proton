using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proton.Server.Core.Models;

namespace Proton.Server.Core.Models.Log
{
    public class Session
    {
        public long Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime TimestampLogin { get; set; } = DateTime.UtcNow;
        public DateTime? TimestampLogout { get; set; }
        public string Ipv4 { get; set; } = "";
        public string Ipv6 { get; set; } = "";
        public string Country { get; set; } = "";

        //Foreign key for User 
        public long UserId { get; set; }
        public User? _User { get; set; }
    }
}
