using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Proton.Server.Core.Interfaces;
using Proton.Server.Core.Models;
using Proton.Server.Core.Tables;
using Proton.Server.Core.Tables.Log;

namespace Proton.Server.Infrastructure.Persistence;

public class DefaultDbContext : DbContext, IDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<RaceMap> RaceMaps => Set<RaceMap>();
    public DbSet<RaceStartPoint> RaceStartPoints => Set<RaceStartPoint>();
    public DbSet<RacePoint> RacePoints => Set<RacePoint>();

    public DefaultDbContext() { }
    public DefaultDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DefaultDbContext).Assembly);
    }

    EntityEntry<TEntity> IDbContext.Add<TEntity>(TEntity entity) where TEntity : class
    {
        return Add(entity);
    }

    ValueTask<EntityEntry<TEntity>> IDbContext.AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class
    {
        return AddAsync(entity, cancellationToken);
    }

    EntityEntry<TEntity> IDbContext.Attach<TEntity>(TEntity entity) where TEntity : class
    {
        return Attach(entity);
    }

    public void AddRange(params IAggregateRoot[] entities)
    {
        base.AddRange(entities);
    }
}
