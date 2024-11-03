using AltV.Net;
using AltV.Net.Elements.Entities;

namespace Proton.Server.Resource.Features.Vehicles;

public sealed class ProtonVehicleFactory : IEntityFactory<IVehicle>
{
    public IVehicle Create(ICore core, nint entityPointer, uint id)
    {
        return new ProtonVehicle(core, entityPointer, id);
    }
}
