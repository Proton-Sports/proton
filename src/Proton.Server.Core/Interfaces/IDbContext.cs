using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Proton.Server.Core.Interfaces;

public interface IDbContext
{
    EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class, IAggregateRoot;
    ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class, IAggregateRoot;
    EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class, IAggregateRoot;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
}
