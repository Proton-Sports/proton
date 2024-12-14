using Proton.Server.Resource.Features.Ipls;
using Proton.Server.Resource.Features.Ipls.Abstractions;
using Proton.Server.Resource.Features.Ipls.Scripts;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddIplFeatures(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddOptions<IplOptions>()
            .BindConfiguration(IplOptions.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        serviceCollection.AddSingleton<IIplService, IplService>().AddHostedService<UnloadIplsScript>();
        return serviceCollection;
    }
}
