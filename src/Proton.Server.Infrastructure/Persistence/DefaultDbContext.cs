using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;

namespace Proton.Server.Infrastructure.Persistence;

public class DefaultDbContext : DbContext, IDbContext
{
    public DefaultDbContext() { }
    public DefaultDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DefaultDbContext).Assembly);
    }
}