using AltV.Net;
using AltV.Net.Async.Elements.Entities;
using Proton.Server.Resource.Features.Vehicles.Abstractions;

namespace Proton.Server.Resource.Features.Vehicles;

public sealed class ProtonVehicle(ICore core, nint nativePointer, uint id)
    : AsyncVehicle(core, nativePointer, id),
        IProtonVehicle
{
    public long GarageId { get; set; }
    public bool AdminPanelFlag { get; set; }
}
