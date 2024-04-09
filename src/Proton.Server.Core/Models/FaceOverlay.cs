using Microsoft.EntityFrameworkCore;

namespace Proton.Server.Core.Models;
[Keyless]
public class FaceOverlay
{
    public byte Index { get; set; }
    public int Value { get; set;}
    public float Opacity { get; set; }
    public bool HasColor { get; set;}
    public byte FirstColor { get; set;}
}