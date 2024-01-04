using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Proton.Server.Core;
using Proton.Server.Infrastructure.Persistence;

namespace Proton.Server.Resource.Persistence;

public class DesignTimeDefaultDbContextFactory : IDesignTimeDbContextFactory<DefaultDbContext>
{
    public DefaultDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile($"appsettings.Local.json", false)
            .Build();

        var persistenceOptions = configuration.GetSection(PersistenceOptions.Section).Get<PersistenceOptions>()!;
        var optionsBuilder = new DbContextOptionsBuilder<DefaultDbContext>();
        optionsBuilder
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
        return new DefaultDbContext(optionsBuilder.Options);
    }
}
