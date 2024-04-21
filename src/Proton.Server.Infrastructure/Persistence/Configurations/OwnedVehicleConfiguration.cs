using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Proton.Server.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Infrastructure.Persistence.Configurations
{
    public sealed class OwnedVehicleConfiguration : IEntityTypeConfiguration<OwnedVehicle>
    {
        public void Configure(EntityTypeBuilder<OwnedVehicle> builder)
        {
            builder.HasKey(x=> x.Id);
        }
    }
}
