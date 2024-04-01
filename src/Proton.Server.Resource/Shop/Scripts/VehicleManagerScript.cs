using AltV.Net;
using AltV.Net.Elements.Entities;
using Proton.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Resource.Shop.Scripts
{
    internal class VehicleManagerScript : IStartup
    {
        private Dictionary<IPlayer, IVehicle> vehicles = new Dictionary<IPlayer, IVehicle>();

        public VehicleManagerScript()
        {
            Alt.OnClient<string>("shop:vehicle:showroom", SpawnCar);
        }

        private void SpawnCar(IPlayer p, string veh)
        {
            if(vehicles.ContainsKey(p))
            {
                vehicles[p].Destroy();
                vehicles.Remove(p);
            }
            var pos = p.Position;
            pos.X += 2;
            pos.Y += 2;
            pos.Z -= 0.5f;

            var vehicle = Alt.CreateVehicle(Alt.Hash(veh),
                pos,
                p.Rotation, 10);
            vehicle.EngineOn = true;
            vehicle.NumberplateText = "M0D7";

            vehicles[p] = vehicle;
        }
    }
}
