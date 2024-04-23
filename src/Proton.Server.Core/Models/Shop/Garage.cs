using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proton.Server.Core.Interfaces;

namespace Proton.Server.Core.Models.Shop
{
    public class Garage : IAggregateRoot
    {
        public long Id { get; set; }
        public long OwnerId { get; set; }
        public User Owner { get; set; } = null!;
        public long VehicleId { get; set; }
        public Vehicle VehicleItem { get; set; } = null!;
        public int AltVColor { get; set; } = 0;
        public DateTime PurchasedDate { get; set; } = DateTime.UtcNow;
    }
}
