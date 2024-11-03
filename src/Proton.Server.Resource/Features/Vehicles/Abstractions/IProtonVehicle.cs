using AltV.Net.Async;
using AltV.Net.Elements.Entities;

namespace Proton.Server.Resource.Features.Vehicles.Abstractions;

public interface IProtonVehicle : IVehicle, IAsyncConvertible<IVehicle>
{
    long GarageId { get; set; }
}
