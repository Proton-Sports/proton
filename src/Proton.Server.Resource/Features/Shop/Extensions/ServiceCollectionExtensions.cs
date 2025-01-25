using Microsoft.Extensions.DependencyInjection;
using Proton.Server.Resource.Features.Shop.Scripts;

namespace Proton.Server.Resource.Features.Shop.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShops(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddStartup<OutfitScript>()
            .AddStartup<ClothScript>()
            .AddHostedService<TuningShopScript>()
            .AddHostedService<VehicleShopScript>();
        return serviceCollection;
    }
}
