using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Proton.Server.Infrastructure.Authentication;
using Proton.Server.Resource.Authentication.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Resource.Authentication.Extentions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services)
        {
            services.AddStartup<AuthenticationScript>();
            services.AddSingleton<DiscordHandler>();
            return services;
        }
    }
}
