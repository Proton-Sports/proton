using AltV.Net.Client;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;
using AltV.Net.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Client.Infrastructure.Utils
{
    public class Marker
    {
        public delegate void OnEnter(IWorldObject target);
        public delegate void OnLeave(IWorldObject target);

        public event OnEnter? onEnter;
        public event OnLeave? onLeave;

        private readonly IMarker marker;
        private readonly IColShape colShape;

        public Marker(Position pos, MarkerType type, Rgba color)
        {
            marker = Alt.CreateMarker(type, pos, color, true, 100);
            colShape = Alt.CreateColShapeCircle(pos, 2f);

            Alt.OnColShape += Alt_OnColShape;
        }

        private void Alt_OnColShape(IColShape colShape, IWorldObject target, bool state)
        {
            if (this.colShape.Id != colShape.Id) return;

            if (state && onEnter != null)
                onEnter(target);
            else if (!state && onLeave != null)
                onLeave(target);
            else
                Alt.LogWarning($"Marker has no event handler Id: {marker.Id}");
        }
    }
}
