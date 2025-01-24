using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proton.Server.Core.Models;

namespace Proton.Server.Infrastructure.Persistence.Configurations;

public sealed class PlayerVehicleWheelVariationConfiguration : IEntityTypeConfiguration<PlayerVehicleWheelVariation>
{
    public void Configure(EntityTypeBuilder<PlayerVehicleWheelVariation> builder)
    {
        builder.Property(a => a.Id);
        builder.Property(a => a.PlayerVehicleId);
        builder.Property(a => a.WheelVariationId);

        builder.HasOne(a => a.PlayerVehicle).WithMany(a => a.WheelVariations).HasForeignKey(a => a.PlayerVehicleId);
        builder.HasOne(a => a.WheelVariation).WithMany().HasForeignKey(a => a.WheelVariationId);

        builder.HasKey(a => a.Id);
        builder.HasIndex(a => new { a.PlayerVehicleId, a.WheelVariationId }).IsUnique();
    }
}
