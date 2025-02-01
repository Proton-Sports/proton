using Microsoft.EntityFrameworkCore;

namespace Proton.Server.Core.Models;

[Keyless]
public class FaceFeature
{
    public int Index { get; set;}
    public float Value { get; set;}
}
