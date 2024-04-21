using Proton.Server.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Core.Models
{
    public class Vehicle : IAggregateRoot
    {
        public long Id { get; set; }
        public string DisplayName { get; set; } = "";
        public string AltVHash { get; set; } = "";
        public int Price { get; set; } = 0;
        public string Category { get; set; } = "";
    }
}
