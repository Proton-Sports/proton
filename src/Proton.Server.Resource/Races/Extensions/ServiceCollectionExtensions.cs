using Proton.Server.Resource.Races.Scripts;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRaces(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddStartup<RaceCreatorScript>()
            .AddStartup<RaceHostScript>();
        return serviceCollection;
    }
}
