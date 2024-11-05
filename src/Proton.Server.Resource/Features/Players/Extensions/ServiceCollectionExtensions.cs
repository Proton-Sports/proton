using Proton.Server.Resource.Features.Players;
using Proton.Server.Resource.Features.Players.Abstractions;
using Proton.Server.Resource.Features.Players.Scripts;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddVehicleFeatures(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IClosetService, ClosestService>()
            .AddHostedService<RewardForPlayingScript>()
            .AddHostedService<ClothesMenuScript>();
        return serviceCollection;
    }
}
