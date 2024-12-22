<<<<<<< HEAD
namespace Proton.Server.Core.Models;

<<<<<<< HEAD:src/Proton.Server.Core/Models/Vehicle.cs
public class Vehicle
{
    public long Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string AltVHash { get; set; } = string.Empty;
    public int Price { get; set; } = 0;
=======
=======
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1
namespace Proton.Server.Core.Models.Shop
{
    public class Vehicle
    {
        public long Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string AltVHash { get; set; } = string.Empty;
        public int Price { get; set; } = 0;
        public string Category { get; set; } = "";

        public ICollection<Garage> Garages { get; set; } = [];
    }
<<<<<<< HEAD
>>>>>>> 10f8164571fb7aec57ac8c49f85f305ccbd1793a:src/Proton.Server.Core/Models/Shop/Vehicle.cs
=======
>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1
}
