using Proton.Server.Resource.Features.Tuning.Scripts;

namespace Microsoft.Extensions.DependencyInjection;
public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddTuningFeature(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddHostedService<TuningScript>();
        return serviceCollection;
    }
}

