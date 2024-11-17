using AltV.Net;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Infrastructure.Persistence;

namespace Proton.Server.Infrastructure.Services
{
    public class OutfitService
    {
        private readonly IDbContextFactory<DefaultDbContext> dbContext;

        public OutfitService(IDbContextFactory<DefaultDbContext> dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task EquipPlayerClothes(PPlayer p)
        {
            var db = await dbContext.CreateDbContextAsync();

            var user = db
                .Users.Where(x => x.Id == p.ProtonId)
                .Include(x => x.Closets)
                .ThenInclude(x => x.ClothItem)
                .FirstOrDefault();

            if (user == null)
            {
                Alt.LogWarning($"Cannot fetch user, {p.Name}");
                return;
            }
            var cloths = user.Closets.ToList();

            p.ClearClothing();

            foreach (var cloth in cloths)
            {
                if (cloth.IsEquiped)
                    SetCloth(p, cloth.ClothItem);
            }
        }

        public void SetCloth(IPlayer p, Core.Models.Shop.Cloth cloth)
        {
            uint dlcHash = 0;
            if (cloth.IsDlc)
                dlcHash = Alt.Hash($"mp_m_{cloth.DlcName}");

            if (cloth.IsProp)
            {
                p.SetDlcProps(
                    Convert.ToByte(cloth.Component),
                    ushort.Parse(cloth.Drawable.ToString()),
                    Convert.ToByte(cloth.Texture),
                    dlcHash
                );

                return;
            }

            p.SetDlcClothes(
                Convert.ToByte(cloth.Component),
                ushort.Parse(cloth.Drawable.ToString()),
                Convert.ToByte(cloth.Texture),
                Convert.ToByte(cloth.Palette),
                dlcHash
            );
        }
    }
}
