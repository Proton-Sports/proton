using Proton.Server.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Core.Models.Shop
{
    public class Closet : IAggregateRoot
    {
        public long Id { get; set; }

        public long OwnerId { get; set; }
        public long ClothId { get; set; }

        public User Owner { get; set; } = null!;
        public Cloth ClothItem { get; set; } = null!;
        public bool IsEquiped { get; set; } = false;

        public DateTime PurchaseTimestamp {  get; set; } = DateTime.UtcNow;
    }
}
