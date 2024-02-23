using AltV.Net;
using AltV.Net.Elements.Entities;
using Proton.Server.Core.Factorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
