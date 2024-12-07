using CnR.Client.Features.Games.Abstractions;
using Proton.Client.Resource.Features.Games;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameFeatures(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IGame, Game>();
        return serviceCollection;
    }
}
