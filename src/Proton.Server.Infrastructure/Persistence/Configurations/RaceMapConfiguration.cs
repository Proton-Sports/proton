using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proton.Server.Core.Models;

namespace Proton.Server.Infrastructure.Persistence.Configurations;

public sealed class RaceMapConfiguration : IEntityTypeConfiguration<RaceMap>
{
    public void Configure(EntityTypeBuilder<RaceMap> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(64);
        builder.Property(x => x.IplName).HasMaxLength(64);
    }
}
