using Proton.Server.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Core.Models.Shop
{
    public class Cloth : IAggregateRoot
    {
        public long Id { get; set; }

        public bool IsDlc { get; set; }
        public bool IsProp { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public int Component { get; set; } = 0;
        public int Drawable { get; set; }
        public int Texture { get; set; }
        public int Palette {  get; set; }
        public string DlcName { get; set; } = string.Empty;

        public int Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public ICollection<Closet> Closets { get; set; } = [];
    }
}
