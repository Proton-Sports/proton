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

        public int Component { get; set; }
        public int Drawable { get; set; }
        public int Texture { get; set; }
        public int Palette {  get; set; }
        public string DlcName { get; set; } = string.Empty;

        public int Price { get; set; }
        public ClothType CurrentClothType { get; set; }
        public ICollection<Closet> Closets { get; set; } = [];

        public enum ClothType
        {
            Head,Torso,Pants,Shoes
        }
    }
}
