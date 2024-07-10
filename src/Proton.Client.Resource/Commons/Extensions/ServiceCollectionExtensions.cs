using Proton.Client.Resource.Commons.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddHostedService<TImplementation>(
        this IServiceCollection serviceCollection
    )
        where TImplementation : class, IHostedService
    {
        return serviceCollection.AddSingleton<IHostedService, TImplementation>();
    }
}
