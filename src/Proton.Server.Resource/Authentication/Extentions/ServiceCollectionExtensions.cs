using Microsoft.Extensions.DependencyInjection;
using Proton.Server.Infrastructure.Authentication;
using Proton.Server.Resource.Authentication.Scripts;

namespace Proton.Server.Resource.Authentication.Extentions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services)
        {
            services.AddHostedService<AuthenticationScript>();
            services.AddSingleton<DiscordHandler>();
            return services;
        }
    }
}
