using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Resource.CharacterCreator.Scripts;
using Proton.Client.Resource.Hud.Scripts;

namespace Proton.Client.Resource.Hud.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHud(this IServiceCollection services)
    {
        services.AddStartup<HudScript>();
        return services;
    }
}
