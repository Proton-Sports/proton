using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Proton.Server.Core.Models.Shop;

namespace Proton.Server.Infrastructure.Persistence.Configurations;

public sealed class PlayerVehicleConfiguration : IEntityTypeConfiguration<PlayerVehicle>
{
    public void Configure(EntityTypeBuilder<PlayerVehicle> builder)
    {
        builder.Property(a => a.Model).HasConversion<EnumToNumberConverter<VehicleModel, uint>>();
        builder.HasOne(g => g.Player).WithMany(x => x.Vehicles).HasForeignKey(g => g.PlayerId);
    }
}
