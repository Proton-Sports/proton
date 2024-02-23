using AltV.Net;
using AltV.Net.Elements.Entities;
using Proton.Server.Core.Factorys;

namespace Proton.Server.Infrastructure.Factorys
{
    public class PPlayerFactory : IEntityFactory<IPlayer>
    {
        public IPlayer Create(ICore core, nint entityPointer, uint id)
        {
            return new PPlayer(core, entityPointer, id);
        }
    }
}
