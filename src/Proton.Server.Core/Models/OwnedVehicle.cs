namespace Proton.Server.Core.Models;

public class OwnedVehicle : Vehicle
{
    public string ColorDisplayname { get; set; } = string.Empty;
    public string AltVColor { get; set; } = string.Empty;
    public DateTime PurchasedDate { get; set; } = DateTime.UtcNow;

    public ICollection<OwnedVehicleTuning> Tunings { get; set; } = new List<OwnedVehicleTuning>();
}
