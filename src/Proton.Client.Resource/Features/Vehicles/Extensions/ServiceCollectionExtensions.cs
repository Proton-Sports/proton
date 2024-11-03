using Proton.Client.Resource.Features.Vehicles.Scripts;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddVehicleFeatures(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<VehicleMenuScript>();
        return serviceCollection;
    }
}
