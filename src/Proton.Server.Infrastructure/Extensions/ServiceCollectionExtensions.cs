using Microsoft.Extensions.Configuration;
using Proton.Server.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        AddPersistenceOptions(serviceCollection, configuration);
        return serviceCollection;
    }

    private static IServiceCollection AddPersistenceOptions(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection
            .AddOptions<PersistenceOptions>()
            .Bind(configuration.GetSection(PersistenceOptions.Section))
            .ValidateDataAnnotations();
        return serviceCollection;
    }
}
