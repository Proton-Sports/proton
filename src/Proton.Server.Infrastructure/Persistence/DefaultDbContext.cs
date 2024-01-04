using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Core.Tables;
using Proton.Server.Core.Tables.Log;

namespace Proton.Server.Infrastructure.Persistence;

public class DefaultDbContext : DbContext, IDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }

    public DefaultDbContext() { }
    public DefaultDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DefaultDbContext).Assembly);
    }
}