using System.ComponentModel.DataAnnotations;
using AltV.Net.Data;
using Microsoft.Extensions.Configuration;
using Proton.Server.Resource.Features.Ipls;
using Proton.Server.Resource.Features.Ipls.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddIplFeatures(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddOptions<IplOptions>()
            .BindConfiguration(IplOptions.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        serviceCollection.AddSingleton<IIplService, IplService>();
        return serviceCollection;
    }
}
