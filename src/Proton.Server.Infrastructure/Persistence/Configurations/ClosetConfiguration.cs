using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proton.Server.Core.Models;
using Proton.Server.Core.Models.Shop;

namespace Proton.Server.Infrastructure.Persistence.Configurations
{
    public sealed class ClosetConfiguration : IEntityTypeConfiguration<Closet>
    {
        public void Configure(EntityTypeBuilder<Closet> builder)
        {
            builder.HasOne<User>(g => g.Owner)
                .WithMany(x => x.Closets)
                .HasForeignKey(g => g.OwnerId);

            builder.HasOne<Cloth>(g => g.ClothItem)
                .WithMany(x => x.Closets)
                .HasForeignKey(g => g.ClothId);
        }
    }
}
