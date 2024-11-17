using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Proton.Server.Core.Interfaces;
using Proton.Server.Core.Models;
using Proton.Server.Core.Models.Log;
using Proton.Server.Core.Models.Shop;

namespace Proton.Server.Infrastructure.Persistence;

public class DefaultDbContext : DbContext, IDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<RaceMap> RaceMaps => Set<RaceMap>();
    public DbSet<RaceStartPoint> RaceStartPoints => Set<RaceStartPoint>();
    public DbSet<RacePoint> RacePoints => Set<RacePoint>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<UserRaceRestoration> UserRaceRestorations => Set<UserRaceRestoration>();
<<<<<<< HEAD
    public DbSet<VehicleTuning> VehicleTunings => Set<VehicleTuning>();
=======
    public DbSet<Garage> Garages => Set<Garage>();
    public DbSet<Cloth> Cloths => Set<Cloth>();
    public DbSet<Closet> Closets => Set<Closet>();
    public DbSet<BanRecord> BanRecords => Set<BanRecord>();
>>>>>>> 10f8164571fb7aec57ac8c49f85f305ccbd1793a

    public DefaultDbContext() { }

    public DefaultDbContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DefaultDbContext).Assembly);
    }

    EntityEntry<TEntity> IDbContext.Add<TEntity>(TEntity entity)
        where TEntity : class
    {
        return Add(entity);
    }

    ValueTask<EntityEntry<TEntity>> IDbContext.AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
        where TEntity : class
    {
        return AddAsync(entity, cancellationToken);
    }

    EntityEntry<TEntity> IDbContext.Attach<TEntity>(TEntity entity)
        where TEntity : class
    {
        return Attach(entity);
    }

    public void AddRange(params IAggregateRoot[] entities)
    {
        base.AddRange(entities);
    }
}
