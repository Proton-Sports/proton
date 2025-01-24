using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Proton.Server.Core.Models;
using Proton.Server.Core.Models.Log;
using Proton.Server.Core.Models.Shop;

namespace Proton.Server.Core.Interfaces;

public interface IDbContext : IDisposable, IAsyncDisposable
{
    DbSet<User> Users { get; }
    DbSet<Character> Characters { get; }
    DbSet<Session> Sessions { get; }
    DbSet<StockVehicle> StockVehicles { get; }
    DbSet<RaceMap> RaceMaps { get; }
    DbSet<RaceStartPoint> RaceStartPoints { get; }
    DbSet<RacePoint> RacePoints { get; }
    DbSet<UserRaceRestoration> UserRaceRestorations { get; }
    DbSet<PlayerVehicle> PlayerVehicles { get; }
    DbSet<Closet> Closets { get; }
    DbSet<Cloth> Cloths { get; }
    DbSet<BanRecord> BanRecords { get; }
    DbSet<Mod> Mods { get; }
    DbSet<PlayerVehicleMod> PlayerVehicleMods { get; }
    DbSet<PlayerVehicleActiveMod> PlayerVehicleActiveMods { get; }
    DbSet<WheelVariation> WheelVariations { get; }
    DbSet<PlayerVehicleWheelVariation> PlayerVehicleWheelVariations { get; }
    DbSet<PlayerVehicleActiveWheelVariation> PlayerVehicleActiveWheelVariations { get; }

    DatabaseFacade Database { get; }

    EntityEntry<TEntity> Add<TEntity>(TEntity entity)
        where TEntity : class, IAggregateRoot;
    ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IAggregateRoot;
    void AddRange(params IAggregateRoot[] entities);
    EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
        where TEntity : class, IAggregateRoot;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
}
