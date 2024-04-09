using Proton.Client.Resource.CharacterCreator.Models;

namespace Proton.Server.Resource.CharacterCreator.Models;

public class Character
{
    public int Id { get; set; }
    public int CharacterGender { get; set; }
    public int FaceFather { get; set; }
    public int FaceMother { get; set; }
    public int SkinFather { get; set; }
    public int SkinMother { get; set; }
    public float SkinMix { get; set; }
    public float FaceMix { get; set; }
    public int EyeColor { get; set; }
    public List<FaceFeature> FaceFeatures { get; set; } = new();
    public List<FaceOverlay> FaceOverlays { get; set; } = new();
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