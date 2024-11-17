namespace Proton.Server.Core.Models;
public class OwnedVehicleTuning
{
    public long Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string Value { get; set; } = string.Empty;
    public bool isEquiped { get; set; }
}
