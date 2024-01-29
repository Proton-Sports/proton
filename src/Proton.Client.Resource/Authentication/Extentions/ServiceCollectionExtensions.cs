
using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Resource.Authentication.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Client.Resource.Authentication.Extentions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services)
    {
        services.AddStartup<AuthenticationScript>();
        return services;
    }
}
