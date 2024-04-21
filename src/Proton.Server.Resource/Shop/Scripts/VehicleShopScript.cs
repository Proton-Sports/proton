using Proton.Server.Infrastructure.Interfaces;
using Proton.Server.Infrastructure.Services;
using Proton.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Resource.Shop.Scripts
{
    internal class VehicleShopScript(IShop service) : IStartup
    {
        private readonly IShop service = service;
    }
}
