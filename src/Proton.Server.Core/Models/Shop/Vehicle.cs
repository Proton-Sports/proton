using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Core.Models.Shop
{
    public class Vehicle
    {
        public long Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string AltVHash { get; set; } = string.Empty;
        public int Price { get; set; } = 0;
        public string Category { get; set; } = "";

        public ICollection<Garage> Garages { get; set; } = [];
    }
}
