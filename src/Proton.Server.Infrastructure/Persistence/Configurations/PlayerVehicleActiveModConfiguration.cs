using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proton.Server.Core.Models;

namespace Proton.Server.Infrastructure.Persistence.Configurations;

public sealed class PlayerVehicleActiveModConfiguration : IEntityTypeConfiguration<PlayerVehicleActiveMod>
{
    public void Configure(EntityTypeBuilder<PlayerVehicleActiveMod> builder)
    {
        builder.Property(a => a.PlayerVehicleModId);

        builder
            .HasOne(a => a.PlayerVehicleMod)
            .WithOne(a => a.PlayerVehicleActiveMod)
            .HasForeignKey<PlayerVehicleActiveMod>(a => a.PlayerVehicleModId);

        builder.HasKey(a => a.PlayerVehicleModId);
    }
}
