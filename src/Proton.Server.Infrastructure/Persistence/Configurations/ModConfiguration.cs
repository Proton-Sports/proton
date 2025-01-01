using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Proton.Server.Core.Models;

namespace Proton.Server.Infrastructure.Persistence.Configurations;

public sealed class ModConfiguration : IEntityTypeConfiguration<Mod>
{
    public void Configure(EntityTypeBuilder<Mod> builder)
    {
        builder.Property(a => a.Id);
        builder.Property(a => a.Category);
        builder.Property(a => a.Model).HasConversion<EnumToNumberConverter<VehicleModel, uint>>();
        builder.Property(a => a.Value);
        builder.Property(a => a.Price);

        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.Category);
        builder.HasIndex(a => a.Model);
    }
}
