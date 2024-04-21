
using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Resource.Authentication.Scripts;
using Proton.Client.Resource.Shop.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Client.Resource.Shop.Extentions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShop(this IServiceCollection services)
    {
        services.AddStartup<VehicleScript>();
        return services;
    }
}
