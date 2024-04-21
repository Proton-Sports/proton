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
            .AddStartup<RaceCreatorScript>()
            .AddStartup<RaceMenuScript>()
            .AddStartup<RaceMenuRacesTabScript>()
            .AddStartup<RaceHostScript>()
            .AddStartup<RaceCountdownScript>()
            .AddStartup<RacePrepareScript>()
            .AddStartup<RaceStartScript>()
            .AddStartup<RaceEndScript>()
            .AddStartup<RaceLeaveScript>();
        return serviceCollection;
    }
}
