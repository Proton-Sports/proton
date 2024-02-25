using Microsoft.Extensions.DependencyInjection;
using Proton.Server.Infrastructure.Interfaces;
using Proton.Server.Infrastructure.Services;
using Proton.Server.Resource.Shop.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Resource.Shop.Extentions
{
    internal static class ServiceCollectionExtention
    {
        public static IServiceCollection AddShops(this IServiceCollection services)
        {           
            services.AddSingleton<IShop, VehicleShopService>();
            services.AddStartup<VehicleShopScript>();
            return services;
        }
    }
}
