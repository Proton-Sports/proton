using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Proton.Server.Core.Models;
using Proton.Shared.Constants;

namespace Proton.Server.Infrastructure.Persistence.Configurations;

public sealed class WheelVariationConfiguration : IEntityTypeConfiguration<WheelVariation>
{
    public void Configure(EntityTypeBuilder<WheelVariation> builder)
    {
        builder.Property(a => a.Id);
        builder.Property(a => a.Type).HasConversion<EnumToNumberConverter<WheelType, byte>>();
        builder.Property(a => a.Name).HasMaxLength(64);
        builder.Property(a => a.Model).HasConversion<EnumToNumberConverter<VehicleModel, uint>>();
        builder.Property(a => a.Value);
        builder.Property(a => a.Price);

        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.Type);
        builder.HasIndex(a => a.Model);
    }
}
