using Proton.Shared.Interfaces;
using Proton.Shared.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStartup<TImplementation>(this IServiceCollection serviceCollection) where TImplementation : class, IStartup
    {
        serviceCollection.AddSingleton<IStartup, TImplementation>();
        serviceCollection.AddTransient<IIncrementalCounter, IncrementalCounter>();
        return serviceCollection;
    }
    public static IServiceCollection AddStartup<TImplementation>(this IServiceCollection serviceCollection, Func<IServiceProvider, TImplementation> implementationFactory) where TImplementation : class, IStartup
    {
        serviceCollection.AddSingleton<IStartup, TImplementation>(implementationFactory);
        return serviceCollection;
    }
}
