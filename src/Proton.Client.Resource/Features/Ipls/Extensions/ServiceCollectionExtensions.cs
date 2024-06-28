using Proton.Client.Resource.Features.Ipls;
using Proton.Client.Resource.Features.Ipls.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddIplFeatures(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IIplService, IplService>();
        return serviceCollection;
    }
}
