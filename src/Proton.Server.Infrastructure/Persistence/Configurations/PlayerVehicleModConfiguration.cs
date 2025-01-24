using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proton.Server.Core.Models;

namespace Proton.Server.Infrastructure.Persistence.Configurations;

public sealed class PlayerVehicleModConfiguration : IEntityTypeConfiguration<PlayerVehicleMod>
{
    public void Configure(EntityTypeBuilder<PlayerVehicleMod> builder)
    {
        builder.Property(a => a.Id);
        builder.Property(a => a.PlayerVehicleId);
        builder.Property(a => a.ModId);

        builder.HasOne(a => a.PlayerVehicle).WithMany(a => a.Mods).HasForeignKey(a => a.PlayerVehicleId);
        builder.HasOne(a => a.Mod).WithMany().HasForeignKey(a => a.ModId);

        builder.HasKey(a => a.Id);
        builder.HasIndex(a => new { a.PlayerVehicleId, a.ModId }).IsUnique();
    }
}
