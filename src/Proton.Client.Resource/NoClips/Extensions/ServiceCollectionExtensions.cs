using Proton.Client.Resource.NoClips.Scripts;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddNoClips(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddStartup<NoClipScript>();
        return serviceCollection;
    }
}
