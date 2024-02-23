using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Core.Models
{
    public class OwnedVehicle : Vehicle
    {
        public string ColorDisplayname { get; set; } = string.Empty;
        public string AltVColor { get; set; } = string.Empty;
        public DateTime PurchasedDate { get; set; } = DateTime.UtcNow;
    }
}
