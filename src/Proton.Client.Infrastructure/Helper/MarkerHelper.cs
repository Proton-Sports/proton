using AltV.Net.Client;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;
using AltV.Net.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Client.Infrastructure.Helper
{
    public class MarkerHelper
    {
        private readonly IMarker marker; 
        private readonly IColShape colShape;

        public delegate void OnEnter();
        public delegate void OnLeave();

        public event OnEnter OnEnterEvent;
        public event OnLeave OnLeaveEvent;

#pragma warning disable CS8618
        public MarkerHelper(MarkerType type, Position pos, Rgba color)
#pragma warning restore CS8618
        {
            marker = Alt.CreateMarker(type, pos, color, false, 0);
            colShape = Alt.CreateColShapeCircle(pos, 3);

            Alt.OnColShape += Alt_OnColShape;
        }

        private void Alt_OnColShape(IColShape colShape, IWorldObject target, bool state)
        {
            if (colShape.Id != colShape.Id ||
                target is IPlayer) return;

            if (state)
                OnEnterEvent?.Invoke();
            else
                OnLeaveEvent?.Invoke();
        }
    }
}
