using System.Text.Json;
using System.Text.Json.Serialization;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Proton.Server.Core.Factorys;
using Proton.Server.Core.Models;
using Proton.Server.Infrastructure.CharacterCreator;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource.CharacterCreator.Scripts;

public class CharacterCreatorScript : IStartup
{
    private readonly CharacterHandler characterHandler;
    public CharacterCreatorScript(CharacterHandler characterHandler)
    {
        this.characterHandler = characterHandler;
        
        AltAsync.OnClient<IPlayer>("server:testCharCreator", StartCharacterCreator);
        AltAsync.OnClient<IPlayer, string>("characterServer:setAppearance", SetAppearance);

        AltAsync.OnServer<IPlayer>("auth:firstSignIn", CheckAppearance);
    }

    private async void CheckAppearance(IPlayer player)
    {
        if (player is not PPlayer protonPlayer) return;
        if (!protonPlayer.Exists) return;
        if (protonPlayer.ProtonId == -1) return;

        var hasCharacter = await characterHandler.HasCharacter(protonPlayer.ProtonId);
        if (hasCharacter)
        {
            var userCharacter = await characterHandler.GetByUserId(protonPlayer.ProtonId);
            SetAppearance(player, JsonSerializer.Serialize(userCharacter));
        }
        else
        {
            StartCharacterCreator(player);
        }
    }

    private async void SetAppearance(IPlayer player, string appearanceJson)
    {
        var isValid = player.Exists;
        if (!isValid) return;
        
        var characterAppearance = JsonSerializer.Deserialize<Character>(appearanceJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        });
        
        if (characterAppearance == null)
        {
            player.Kick("Invalid appearance. Please try again.");
            return;
        }

        if (player is PPlayer protonPlayer)
            characterAppearance.UserId =
                protonPlayer.ProtonId;

        await characterHandler.Add(characterAppearance);
        
        player.Dimension = 0;
        player.Frozen = false;
        player.Position = new Position(0, 0, 72);
        player.Rotation = Rotation.Zero;
        player.Visible = true;
        
        player.Model = characterAppearance.CharacterGender switch
        {
            0 => (uint)PedModel.FreemodeFemale01,
            1 => (uint)PedModel.FreemodeMale01,
            _ => player.Model
        };

        player.SetHeadBlendData((uint)characterAppearance.FaceFather, (uint)characterAppearance.FaceMother,
            0, (uint)characterAppearance.SkinFather, (uint)characterAppearance.SkinMother, 0, 
            characterAppearance.FaceMix, characterAppearance.SkinMix, 0);
        
        foreach (var characterAppearanceFaceFeature in characterAppearance.FaceFeatures)
        {
            player.SetFaceFeature((byte)characterAppearanceFaceFeature.Index, characterAppearanceFaceFeature.Value);
        }
        
        foreach (var characterAppearanceFaceOverlay in characterAppearance.FaceOverlays)
        {
            player.SetHeadOverlay(characterAppearanceFaceOverlay.Index, (byte)characterAppearanceFaceOverlay.Value, characterAppearanceFaceOverlay.Opacity);

            if (characterAppearanceFaceOverlay.HasColor)
            {
                player.SetHeadOverlayColor(characterAppearanceFaceOverlay.Index, 2, characterAppearanceFaceOverlay.FirstColor, characterAppearanceFaceOverlay.FirstColor);
            }
        }
        
        player.SetClothes(2, (ushort)characterAppearance.HairDrawable, 0, 0);
        player.SetHeadOverlay(1, (byte)characterAppearance.FacialHair, characterAppearance.FacialHairOpacity);
        player.SetHeadOverlayColor(1, 1, (byte)characterAppearance.FirstFacialHairColor, (byte)characterAppearance.SecondFacialHairColor);
        player.SetHeadOverlay(2, (byte)characterAppearance.Eyebrows, 1);
        player.SetHeadOverlayColor(2, 1, (byte)characterAppearance.EyebrowsColor, (byte)characterAppearance.EyebrowsColor);
        player.SetEyeColor((ushort)characterAppearance.EyeColor);
        
        player.HairColor = (byte)characterAppearance.FirstHairColor;
        player.HairHighlightColor = (byte)characterAppearance.SecondHairColor;
        
        player.Emit("characterClient:stopCharacterCreator");
    }

    private void StartCharacterCreator(IPlayer player)
    {
        player.Dimension = (int)(int.MaxValue - player.Id);
        player.Frozen = true;
        player.Position = new Position(402.90833f, -996.61365f, -99.00013f);
        player.Rotation = new Rotation(0, 0, 3.14f);
        player.Visible = false;
        
        player.Emit("characterClient:startCharacterCreator");
    }
}