using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Proton.Server.Core.Models;

namespace Proton.Server.Infrastructure.Persistence.Configurations;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public class FaceFeatureConverter(ConverterMappingHints? mappingHints = null)
        : ValueConverter<List<FaceFeature>, string>(v => Serialize(v),
            v => Deserialize(v),
            mappingHints)
    {
        public static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static string Serialize(List<FaceFeature> value)
        {
            return JsonSerializer.Serialize(value, JsonSerializerOptions);
        }

        public static List<FaceFeature> Deserialize(string value)
        {
            return JsonSerializer.Deserialize<List<FaceFeature>>(value, JsonSerializerOptions) ?? new List<FaceFeature>();
        }
    }
    
    public class FaceOverlayConverter(ConverterMappingHints? mappingHints = null)
        : ValueConverter<List<FaceOverlay>, string>(v => Serialize(v),
            v => Deserialize(v),
            mappingHints)
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static string Serialize(List<FaceOverlay> value)
        {
            return JsonSerializer.Serialize(value, JsonSerializerOptions);
        }

        public static List<FaceOverlay> Deserialize(string value)
        {
            return JsonSerializer.Deserialize<List<FaceOverlay>>(value, JsonSerializerOptions) ?? new List<FaceOverlay>();
        }
    }
    
    public class FaceFeatureComparer() : ValueComparer<List<FaceFeature>>(
        (firstList, secondList) => firstList != null && secondList != null && firstList.SequenceEqual(secondList),
        faceFeatureList => faceFeatureList.Aggregate(0,
            (hashCode, faceFeature) => HashCode.Combine(hashCode, faceFeature.GetHashCode())),
        faceFeatureList => faceFeatureList.ToList());
    
    public class FaceOverlayComparer() : ValueComparer<List<FaceOverlay>>(
        (firstList, secondList) => firstList != null && secondList != null && firstList.SequenceEqual(secondList),
        faceOverlayList => faceOverlayList.Aggregate(0,
            (hashCode, faceOverlay) => HashCode.Combine(hashCode, faceOverlay.GetHashCode())),
        faceOverlayList => faceOverlayList.ToList());
    
    public void Configure(EntityTypeBuilder<Character> modelBuilder)
    {
        modelBuilder.HasKey(p => p.Id);
        modelBuilder.Property(p => p.FaceFather).IsRequired();
        modelBuilder.Property(p => p.FaceMother).IsRequired();
        modelBuilder.Property(p => p.SkinFather).IsRequired();
        modelBuilder.Property(p => p.SkinMother).IsRequired();
        modelBuilder.Property(p => p.SkinMix).IsRequired();
        modelBuilder.Property(p => p.FaceMix).IsRequired();
        modelBuilder.Property(p => p.EyeColor).IsRequired();
        modelBuilder.Property(p => p.HairDrawable).IsRequired();
        modelBuilder.Property(p => p.FirstHairColor).IsRequired();
        modelBuilder.Property(p => p.SecondHairColor).IsRequired();
        modelBuilder.Property(p => p.FacialHair).IsRequired();
        modelBuilder.Property(p => p.FirstFacialHairColor).IsRequired();
        modelBuilder.Property(p => p.SecondFacialHairColor).IsRequired();
        modelBuilder.Property(p => p.FacialHairOpacity).IsRequired();
        modelBuilder.Property(p => p.Eyebrows).IsRequired();
        modelBuilder.Property(p => p.EyebrowsColor).IsRequired();
        modelBuilder
            .HasOne(e => e.User)
            .WithOne(e => e.Character)
            .HasForeignKey<Character>(e => e.UserId)
            .IsRequired();
        modelBuilder
            .Property(e => e.FaceFeatures)
            .IsRequired()
            .HasConversion(new FaceFeatureConverter())
            .Metadata.SetValueComparer(new FaceFeatureComparer());;
        modelBuilder
            .Property(e => e.FaceOverlays)
            .IsRequired()
            .HasConversion(new FaceOverlayConverter())
            .Metadata.SetValueComparer(new FaceOverlayComparer());
    } 
}