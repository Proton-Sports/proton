using AltV.Net;
using AltV.Net.Elements.Entities;

namespace Proton.Server.Core.Factorys
{
    public class PPlayer : Player
    {
        //If -1 Player not Loggedin
        public long ProtonId { get; set; } = -1;

        public PPlayer(ICore core, nint nativePointer, uint id) : base(core, nativePointer, id)
        {
        }
    }
}
