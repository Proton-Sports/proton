using AltV.Net;
using AltV.Net.Elements.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Core.Factorys
{
    public class PPlayer : Player
    {
        //If -1 Player not Loggedin
        public int ProtonId { get; set; } = -1;

        public PPlayer(ICore core, nint nativePointer, uint id) : base(core, nativePointer, id)
        {            
        }
    }
}
