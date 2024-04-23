using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proton.Server.Core.Models;
using Proton.Server.Core.Models.Shop;

namespace Proton.Server.Infrastructure.Persistence.Configurations
{
    public sealed class GarageConfiguration : IEntityTypeConfiguration<Garage>
    {
        public void Configure(EntityTypeBuilder<Garage> builder)
        {
            builder.HasOne<User>(g => g.Owner)
                .WithMany(x => x.Garages)
                .HasForeignKey(g => g.OwnerId);

            builder.HasOne<Vehicle>(g => g.VehicleItem)
                .WithMany(x => x.Garages)
                .HasForeignKey(g => g.VehicleId);
        }
    }
}
