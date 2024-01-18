using Proton.Client.Resource.Races.Scripts;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddRaces(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddStartup<RaceCreatorScript>();
        return serviceCollection;
    }
}
