using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Resource.Features.Shop.Scripts;

namespace Proton.Client.Resource.Features.Shop.Extentions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShops(this IServiceCollection services)
    {
        services.AddStartup<VehicleShop>().AddStartup<ClothShop>().AddHostedService<TuningShopScript>();
        return services;
    }
}
