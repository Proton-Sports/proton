using Proton.Server.Resource.Features.Races;
using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Server.Resource.Features.Races.Scripts;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddRaceFeatures(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IRaceService, DefaultRaceService>()
            .AddSingleton<IRacePointResolver, RacePointLapResolver>()
            .AddSingleton<IRacePointResolver, RacePointRallyResolver>()
            .AddSingleton<IMapCache, MapCache>()
            .AddStartup<RaceScript>()
            .AddHostedService<RaceCreatorScript>()
            .AddHostedService<RaceHostScript>()
            .AddStartup<RaceMenuRacesTabScript>()
            .AddHostedService<RaceCountdownScript>()
            .AddHostedService<RacePrepareScript>()
            .AddStartup<RaceStartScript>()
            .AddStartup<RaceDestroyScript>()
            .AddHostedService<RaceEndScript>()
            .AddStartup<RaceLeaveScript>()
            .AddHostedService<RaceHudScript>()
            .AddHostedService<RaceHitScript>();
        return serviceCollection;
    }
}
