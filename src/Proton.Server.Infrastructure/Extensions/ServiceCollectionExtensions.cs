using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Proton.Server.Core;
using Proton.Server.Core.Interfaces;
using Proton.Server.Infrastructure.Interfaces;
using Proton.Server.Infrastructure.Persistence;
using Proton.Server.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection)
    {
        AddPersistence(serviceCollection);
        serviceCollection.AddSingleton<INoClip, DefaultNoClip>();
        serviceCollection.AddTransient<OutfitService>();
        return serviceCollection;
    }

    private static IServiceCollection AddPersistence(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddOptions<PersistenceOptions>()
            .BindConfiguration(PersistenceOptions.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        serviceCollection.AddPooledDbContextFactory<DefaultDbContext>((provider, builder) =>
        {
            var persistenceOptions = provider.GetRequiredService<IOptions<PersistenceOptions>>().Value;
            builder
                .UseNpgsql(persistenceOptions.ConnectionString, x =>
                {
                    x.MigrationsAssembly(persistenceOptions.MigrationsAssemblyName);
                })
#if DEBUG
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .EnableThreadSafetyChecks()
#else
            .EnableThreadSafetyChecks(false)
#endif
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
        serviceCollection.AddSingleton<IDbContextFactory, DefaultDbContextFactory>();
        return serviceCollection;
    }
}
