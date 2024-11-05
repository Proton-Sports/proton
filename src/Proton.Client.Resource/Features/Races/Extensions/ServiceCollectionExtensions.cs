using Proton.Client.Core.Interfaces;
using Proton.Client.Infrastructure.Services;
using Proton.Client.Resource.Features.Races;
using Proton.Client.Resource.Features.Races.Scripts;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddRaceFeatures(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IRaceCreator, LandRaceCreator>()
            .AddSingleton<IRaceService, DefaultRaceService>()
            .AddSingleton<IRacePointResolver, RacePointLapResolver>()
            .AddSingleton<IRacePointResolver, RacePointRallyResolver>()
            .AddHostedService<RaceCreatorScript>()
            .AddHostedService<RaceMenuScript>()
            .AddStartup<RaceMenuRacesTabScript>()
            .AddStartup<RaceHostScript>()
            .AddStartup<RaceCountdownScript>()
            .AddHostedService<RacePrepareScript>()
            .AddStartup<RaceStartScript>()
            .AddHostedService<RaceFinishCountdownScript>()
            .AddStartup<RaceLeaveScript>()
            .AddStartup<RaceHudScript>()
            .AddStartup<RaceHitScript>()
            .AddStartup<RacePhasingScript>()
            .AddHostedService<RaceFinishScript>()
            .AddHostedService<RaceStartCountdownScript>()
            .AddHostedService<RaceDestroyScript>()
            .AddHostedService<RaceRespawnScript>()
            .AddHostedService<RaceMenuCollectionTabScript>();
        return serviceCollection;
    }
}
