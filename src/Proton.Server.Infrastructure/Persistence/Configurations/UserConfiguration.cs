using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proton.Server.Core.Models;

namespace Proton.Server.Infrastructure.Persistence.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(x => x.Sessions);
            builder.HasOne(p => p.Character)
                .WithOne(e => e.User)
                .HasForeignKey<Character>(e => e.UserId)
                .IsRequired(false);
            builder.HasKey(x => x.Id);
        }
    }
}
