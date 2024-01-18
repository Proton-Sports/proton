using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Proton.Server.Core.Models;

namespace Proton.Server.Core.Interfaces;

public interface IDbContext : IDisposable, IAsyncDisposable
{
    DbSet<RaceMap> RaceMaps { get; }
    DbSet<RaceStartPoint> RaceStartPoints { get; }
    DbSet<RacePoint> RacePoints { get; }

    EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class, IAggregateRoot;
    ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class, IAggregateRoot;
    void AddRange(params IAggregateRoot[] entities);
    EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class, IAggregateRoot;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
}
