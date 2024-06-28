using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Resource.Nametags.Scripts;

namespace Proton.Client.Resource.Nametags.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNametags(this IServiceCollection services)
    {
        services.AddStartup<NametagsScript>();
        return services;
    }
}
