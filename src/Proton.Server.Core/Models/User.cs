<<<<<<< HEAD
using Proton.Server.Core.Models.Log;
using Proton.Server.Core.Models.Shop;
=======
﻿using Proton.Server.Core.Models.Log;
using Proton.Server.Core.Models.Shop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1

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
