using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proton.Server.Core.Models;

namespace Proton.Server.Infrastructure.Persistence.Configurations;

public sealed class UserRaceRestorationConfiguration : IEntityTypeConfiguration<UserRaceRestoration>
{
    public void Configure(EntityTypeBuilder<UserRaceRestoration> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.HasOne(x => x.User).WithOne().HasForeignKey<UserRaceRestoration>(x => x.UserId);
        builder.OwnsMany(
            x => x.Points,
            x =>
            {
                x.WithOwner().HasForeignKey(x => x.UserId);
                x.Property(x => x.Index).ValueGeneratedNever();
                x.HasKey(x => new { x.UserId, x.Index });
            }
        );
    }
}
