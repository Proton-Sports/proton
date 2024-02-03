using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proton.Server.Core.Models;

namespace Proton.Server.Infrastructure.Persistence.Configurations;

public sealed class RacePointConfiguration : IEntityTypeConfiguration<RacePoint>
{
    public void Configure(EntityTypeBuilder<RacePoint> builder)
    {
        builder.HasKey(x => new { x.MapId, x.Index });
        builder.Property(x => x.MapId);
        builder.Property(x => x.Index);
        builder.Property(x => x.Radius);
        builder.ComplexProperty(x => x.Position);

        builder.HasOne(x => x.Map).WithMany(x => x.RacePoints).HasForeignKey(x => x.MapId);
    }
}
