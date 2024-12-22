<<<<<<< HEAD
﻿using AltV.Net;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Infrastructure.Persistence;
=======
﻿using AltV.Net.Elements.Entities;
using AltV.Net;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Factorys;
using Proton.Server.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Proton.Server.Core.Models.Shop;
using AltV.Net.Data;
>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1

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

<<<<<<< HEAD
            var user = db
                .Users.Where(x => x.Id == p.ProtonId)
=======
            var user = db.Users.Where(x => x.Id == p.ProtonId)
>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1
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
<<<<<<< HEAD
                if (cloth.IsEquiped)
=======
                if(cloth.IsEquiped)
>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1
                    SetCloth(p, cloth.ClothItem);
            }
        }

        public void SetCloth(IPlayer p, Core.Models.Shop.Cloth cloth)
        {
            uint dlcHash = 0;
<<<<<<< HEAD
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
=======
            if (cloth.IsDlc) dlcHash = Alt.Hash($"mp_m_{cloth.DlcName}");

            if (cloth.IsProp)
            {
                p.SetDlcProps(Convert.ToByte(cloth.Component),
                    ushort.Parse(cloth.Drawable.ToString()),
                    Convert.ToByte(cloth.Texture),
                    dlcHash);
>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1

                return;
            }

<<<<<<< HEAD
            p.SetDlcClothes(
                Convert.ToByte(cloth.Component),
                ushort.Parse(cloth.Drawable.ToString()),
                Convert.ToByte(cloth.Texture),
                Convert.ToByte(cloth.Palette),
                dlcHash
            );
=======
            p.SetDlcClothes(Convert.ToByte(cloth.Component),
                    ushort.Parse(cloth.Drawable.ToString()),
                    Convert.ToByte(cloth.Texture),
                    Convert.ToByte(cloth.Palette),
                    dlcHash);
>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1
        }
    }
}
