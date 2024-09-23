using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Resource.Utils.Scripts;

namespace Proton.Client.Resource.Utils.Extentions;

public static class ServiceCollectionExtention
{
    public static IServiceCollection AddUtils(this IServiceCollection services)
    {
        services.AddStartup<SpeedometerScript>();

        return services;
    }
}
