using Proton.Client.Resource.Features.Tuning.Scripts;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddTuningFeature(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddStartup<TuningScript>();
        return serviceCollection;
    }
}
