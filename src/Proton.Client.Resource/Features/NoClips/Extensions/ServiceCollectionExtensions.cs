using Proton.Client.Resource.Features.NoClips.Scripts;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddNoClips(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<NoClipScript>();
        return serviceCollection;
    }
}
