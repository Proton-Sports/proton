using Proton.Server.Resource.Features.Races;
using Proton.Server.Resource.Features.Races.Scripts;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddRaceFeatures(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IRaceService, DefaultRaceService>()
            .AddStartup<RaceScript>()
            .AddStartup<RaceCreatorScript>()
            .AddStartup<RaceHostScript>()
            .AddStartup<RaceMenuRacesTabScript>()
            .AddStartup<RaceCountdownScript>()
            .AddStartup<RacePrepareScript>()
            .AddStartup<RaceStartScript>()
            .AddStartup<RaceDestroyScript>()
            .AddStartup<RaceEndScript>()
            .AddStartup<RaceLeaveScript>();
        return serviceCollection;
    }
}
