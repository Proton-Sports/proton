using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Core.Tables.Log
{
    public class Session
    {
        [Key]
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime TimestampLogin {  get; set; }
        public DateTime TimestampLogout { get; set; }
        public string Ipv4 { get; set; } = "";
        public string Ipv6 { get; set; } = "";
        public string Country { get; set; } = "";

        //Foreign key for User 
        public int UserId { get; set; }
        public User? _User { get; set; }
    }
}
