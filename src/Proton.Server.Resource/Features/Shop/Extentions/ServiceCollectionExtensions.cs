using Microsoft.Extensions.DependencyInjection;
using Proton.Server.Resource.Features.Shop.Scripts;

namespace Proton.Server.Resource.Features.Shop;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShops(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddStartup<VehicleScript>()
            .AddStartup<OutfitScript>()
            .AddStartup<ClothScript>()
            .AddHostedService<TuningShopScript>();
        return serviceCollection;
    }
}
