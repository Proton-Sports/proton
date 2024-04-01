using Microsoft.Extensions.DependencyInjection;
using Proton.Server.Infrastructure.CharacterCreator;
using Proton.Server.Resource.CharacterCreator.Scripts;

namespace Proton.Server.Resource.CharacterCreator.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCharacterCreator(this IServiceCollection services)
        {
            services.AddStartup<CharacterCreatorScript>();
            services.AddSingleton<CharacterHandler>();
            return services;
        }
    }
}
