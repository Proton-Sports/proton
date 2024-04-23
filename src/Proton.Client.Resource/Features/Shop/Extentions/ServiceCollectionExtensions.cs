
using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Resource.Authentication.Scripts;
using Proton.Client.Resource.Features.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Client.Resource.Features.Shop.Extentions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShops(this IServiceCollection services)
    {
        services.AddStartup<VehicleShop>();
        return services;
    }
}
