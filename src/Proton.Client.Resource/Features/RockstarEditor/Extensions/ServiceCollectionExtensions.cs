using Proton.Client.Resource.Features.RockstarEditor.Scripts;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddRockstarEditorFeatures(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<RockstarEditorScript>();
        return serviceCollection;
    }
}
