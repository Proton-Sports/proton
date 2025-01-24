using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proton.Server.Core.Models;

namespace Proton.Server.Infrastructure.Persistence.Configurations;

public sealed class PlayerVehicleActiveWheelVariationConfiguration
    : IEntityTypeConfiguration<PlayerVehicleActiveWheelVariation>
{
    public void Configure(EntityTypeBuilder<PlayerVehicleActiveWheelVariation> builder)
    {
        builder.Property(a => a.PlayerVehicleWheelVariationId);

        builder
            .HasOne(a => a.PlayerVehicleWheelVariation)
            .WithOne(a => a.PlayerVehicleActiveWheelVariation)
            .HasForeignKey<PlayerVehicleActiveWheelVariation>(a => a.PlayerVehicleWheelVariationId);

        builder.HasKey(a => a.PlayerVehicleWheelVariationId);
    }
}
