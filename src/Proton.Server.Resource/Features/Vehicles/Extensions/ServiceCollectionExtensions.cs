using Proton.Server.Resource.Features.Vehicles;
using Proton.Server.Resource.Features.Vehicles.Abstractions;
using Proton.Server.Resource.Features.Vehicles.Scripts;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlayerFeatures(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IGarageService, GarageService>()
            .AddHostedService<GarageScript>()
            .AddHostedService<VehicleMenuScript>();
        return serviceCollection;
    }
}
