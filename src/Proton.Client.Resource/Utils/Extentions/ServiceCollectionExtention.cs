using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Resource.Utils.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Client.Resource.Utils.Extentions
{
    internal static class ServiceCollectionExtention
    {
        internal static IServiceCollection AddUtils(this IServiceCollection services)
        {
            services.AddStartup<NotificationScript>();

            return services;
        }
    }
}
