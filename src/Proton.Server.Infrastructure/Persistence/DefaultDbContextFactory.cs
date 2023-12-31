using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;

namespace Proton.Server.Infrastructure.Persistence;

public class DefaultDbContextFactory(IDbContextFactory<DefaultDbContext> dbContextFactory) : IDbContextFactory
{
    public IDbContext CreateDbContext() => dbContextFactory.CreateDbContext();
    public async Task<IDbContext> CreateDbContextAsync() => await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
}