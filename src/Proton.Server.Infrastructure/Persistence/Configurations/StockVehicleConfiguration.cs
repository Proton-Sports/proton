using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Proton.Server.Core.Models.Shop;

namespace Proton.Server.Infrastructure.Persistence.Configurations;

public sealed class StockVehicleConfiguration : IEntityTypeConfiguration<StockVehicle>
{
    public void Configure(EntityTypeBuilder<StockVehicle> builder)
    {
        builder.Property(a => a.Model).HasConversion<EnumToNumberConverter<VehicleModel, int>>();
        builder.HasKey(x => x.Id);
    }
}
