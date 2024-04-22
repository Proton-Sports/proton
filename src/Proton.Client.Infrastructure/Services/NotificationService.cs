using AltV.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Client.Infrastructure.Services
{
    public class NotificationService
    {
        //TODO: Load custom images https://natives.altv.mp/0xDFA2EF8E04127DD5
        public void DrawNotification(string Image = "", string Header = "", string Details= "", string Message = "")
        {
            Alt.Natives.BeginTextCommandThefeedPost("STRING");
            Alt.Natives.AddTextComponentSubstringPlayerName(Message);
            Alt.Natives.EndTextCommandThefeedPostMessagetextTu(
                Image.ToUpper(),
                Image.ToUpper(),
                false,
                4,
                Header,
                Details,
                1.0f);
            Alt.Natives.EndTextCommandThefeedPostTicker(false, false);
        }
    }
}
