namespace Proton.Server.Core.Interfaces;

public interface IDbContextFactory
{
    IDbContext CreateDbContext();
    Task<IDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default);
}
