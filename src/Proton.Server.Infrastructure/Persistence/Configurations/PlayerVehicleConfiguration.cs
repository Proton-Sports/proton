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
        builder.ComplexProperty(
            a => a.PrimaryColor,
            b =>
            {
                b.Property(a => a.R);
                b.Property(a => a.G);
                b.Property(a => a.B);
                b.Property(a => a.A);
            }
        );
        builder.ComplexProperty(
            a => a.SecondaryColor,
            b =>
            {
                b.Property(a => a.R);
                b.Property(a => a.G);
                b.Property(a => a.B);
                b.Property(a => a.A);
            }
        );
        builder.HasOne(g => g.Player).WithMany(x => x.Vehicles).HasForeignKey(g => g.PlayerId);
    }
}
