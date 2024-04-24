using AltV.Net;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Elements.Entities;
using Proton.Server.Core.Models;
using Proton.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Core.Factorys
{
    public class PPlayer : AsyncPlayer
    {
        //If -1 Player not Loggedin
        public long ProtonId { get; set; } = -1;

        public PPlayer(ICore core, nint nativePointer, uint id) : base(core, nativePointer, id)
        {
        }

        public void SendNotification(SharedNotification notify)
            => Player.Emit("player:sendNotification", notify);
        
        public void ClearClothing()
        {
            SetClothes(3, 15, 0, 0); // Torso
            SetClothes(11, 15, 0, 0); // Shirt
            SetClothes(8, 15, 0, 0); // Chest Part Of The Shirt
        }
    }
}
