using Proton.Client.Resource.Features.Ipls;
using Proton.Client.Resource.Features.Ipls.Abstractions;
using Proton.Client.Resource.Features.Ipls.Scripts;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddIplFeatures(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IIplService, IplService>().AddHostedService<UnloadIplsScript>();
        return serviceCollection;
    }
}
