using System.ComponentModel.DataAnnotations;
using Proton.Server.Core.Interfaces;

namespace Proton.Server.Core.Models;

public class Character
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public User User { get; set; }
    public int CharacterGender { get; set; }
    public int FaceFather { get; set; }
    public int FaceMother { get; set; }
    public int SkinFather { get; set; }
    public int SkinMother { get; set; }
    public float SkinMix { get; set; }
    public float FaceMix { get; set; }
    public int EyeColor { get; set; }
    public List<FaceFeature> FaceFeatures { get; set; } = [];
    public List<FaceOverlay> FaceOverlays { get; set; } = [];
    public int HairDrawable { get; set; }
    public int FirstHairColor { get; set; }
    public int SecondHairColor { get; set; }
    public int FacialHair { get; set; }
    public int FirstFacialHairColor { get; set; }
    public int SecondFacialHairColor { get; set; }
    public float FacialHairOpacity { get; set; }
    public int Eyebrows { get; set; }
    public int EyebrowsColor { get; set; }
}