using Proton.Client.Infrastructure.Helper;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Shared.Contants;
using Proton.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Client.Resource.Shop.Scripts
{
    internal class VehicleMarkerScript : IStartup
    {
        //private readonly IUiView uiView;

        //public VehicleMarkerScript(IUiView uiView)
        //{
        //    //Todo: Create marker for each selling point    
        //    var m = new MarkerHelper(AltV.Net.Shared.Enums.MarkerType.MarkerBeast,
        //        new AltV.Net.Data.Position(0, 0, 0), new AltV.Net.Data.Rgba(255, 255, 255, 255));

        //    m.OnLeaveEvent += M_OnLeaveEvent;
        //    m.OnEnterEvent += M_OnEnterEvent;
        //    this.uiView = uiView;
        //}

        //private void M_OnEnterEvent()
        //{
        //    uiView.Emit("shop:vehicles:menuStatus", false);
        //    uiView.Mount(Route.VehicleShop);
        //}

        //private void M_OnLeaveEvent()
        //{
        //    uiView.Emit("shop:vehicles:menuStatus", true);
        //    uiView.Unmount(Route.VehicleShop);
        //}
    }
}
