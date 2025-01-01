using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proton.Server.Core.Models;

namespace Proton.Server.Infrastructure.Persistence.Configurations;

public sealed class VehicleModConfiguration : IEntityTypeConfiguration<VehicleMod>
{
    public void Configure(EntityTypeBuilder<VehicleMod> builder)
    {
        builder.Property(a => a.Id);
        builder.Property(a => a.VehicleId);
        builder.Property(a => a.ModId);

        builder.HasOne(a => a.Vehicle).WithMany().HasForeignKey(a => a.VehicleId);
        builder.HasOne(a => a.Mod).WithMany().HasForeignKey(a => a.ModId);

        builder.HasKey(a => a.Id);
        builder.HasIndex(a => new { a.VehicleId, a.ModId }).IsUnique();
    }
}
