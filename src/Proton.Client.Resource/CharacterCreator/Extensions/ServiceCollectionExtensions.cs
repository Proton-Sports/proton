using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Resource.CharacterCreator.Scripts;

namespace Proton.Client.Resource.CharacterCreator.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCharacterCreator(this IServiceCollection services)
    {
        services.AddStartup<CharacterCreatorScript>();
        return services;
    }
}
