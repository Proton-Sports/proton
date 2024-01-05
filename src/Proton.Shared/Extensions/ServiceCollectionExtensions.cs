using Proton.Shared.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStartup<TImplementation>(this IServiceCollection serviceCollection) where TImplementation : class, IStartup
    {
        serviceCollection.AddSingleton<IStartup, TImplementation>();
        return serviceCollection;
    }
    public static IServiceCollection AddStartup<TImplementation>(this IServiceCollection serviceCollection, Func<IServiceProvider, TImplementation> implementationFactory) where TImplementation : class, IStartup
    {
        serviceCollection.AddSingleton<IStartup, TImplementation>(implementationFactory);
        return serviceCollection;
    }
}
