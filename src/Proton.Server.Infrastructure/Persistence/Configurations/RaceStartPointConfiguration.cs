using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proton.Server.Core.Models;

namespace Proton.Server.Infrastructure.Persistence.Configurations;

public sealed class RaceStartPointConfiguration : IEntityTypeConfiguration<RaceStartPoint>
{
    public void Configure(EntityTypeBuilder<RaceStartPoint> builder)
    {
        builder.HasKey(x => new { x.MapId, x.Index });
        builder.Property(x => x.MapId);
        builder.Property(x => x.Index);
        builder.ComplexProperty(x => x.Position);

        builder.HasOne(x => x.Map).WithMany(x => x.StartPoints).HasForeignKey(x => x.MapId);
    }
}
