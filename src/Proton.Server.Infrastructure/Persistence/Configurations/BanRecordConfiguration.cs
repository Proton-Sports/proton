using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Proton.Server.Core.Models;

namespace Proton.Server.Infrastructure.Persistence.Configurations;

public sealed class BanRecordConfiguration : IEntityTypeConfiguration<BanRecord>
{
    public void Configure(EntityTypeBuilder<BanRecord> builder)
    {
        builder.Property(a => a.Kind).HasConversion<EnumToStringConverter<BanKind>>().HasMaxLength(24);
        builder.Property(a => a.Id).HasMaxLength(1024);
        builder.Property(a => a.Name).HasMaxLength(64);
        builder.HasKey(a => a.Id);
    }
}
