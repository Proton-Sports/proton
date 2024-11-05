using Proton.Client.Resource.Features.Players.Scripts;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlayerFeatures(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<HoldToShowCursorScript>().AddHostedService<ClothesMenuScript>();
        return serviceCollection;
    }
}
