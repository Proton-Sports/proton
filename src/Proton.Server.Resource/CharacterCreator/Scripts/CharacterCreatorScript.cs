using System.Text.Json;
using System.Text.Json.Serialization;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Proton.Server.Core.Models;
using Proton.Server.Infrastructure.CharacterCreator;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.Authentication.Scripts;
using Proton.Server.Resource.SharedKernel;

namespace Proton.Server.Resource.CharacterCreator.Scripts;

public class CharacterCreatorScript(CharacterHandler characterHandler) : HostedService
{
    static readonly JsonSerializerOptions serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    public override Task StartAsync(CancellationToken ct)
    {
        AltAsync.OnClient<IPlayer, string>("characterServer:setAppearance", CreateCharacter);
        AuthenticationScript.OnAuthenticationDoneEvent += OnAuthenticationDone;
        return Task.CompletedTask;
    }

    async Task OnAuthenticationDone(IPlayer player)
    {
        if (player is not PPlayer protonPlayer || !protonPlayer.Exists || protonPlayer.ProtonId == -1)
        {
            return;
        }

        var hasCharacter = await characterHandler.HasCharacter(protonPlayer.ProtonId).ConfigureAwait(false);
        if (hasCharacter)
        {
            var userCharacter = await characterHandler.GetByUserId(protonPlayer.ProtonId).ConfigureAwait(false);
            if (userCharacter == null)
            {
                player.Kick("Invalid character. Please try again.");
                return;
            }

            SetAppearance(protonPlayer, userCharacter);
        }
        else
        {
            StartCharacterCreator(player);
        }
    }

    static void SetAppearance(PPlayer player, Character characterAppearance)
    {
        player.Spawn(new Position(551.916f, 5562.336f, -96.042f));
        player.Dimension = 0;
        player.Frozen = true;
        player.Invincible = true;
        player.Rotation = Rotation.Zero;
        player.Visible = true;

        player.Model = characterAppearance.CharacterGender switch
        {
            0 => (uint)PedModel.FreemodeFemale01,
            _ => (uint)PedModel.FreemodeMale01,
        };

        player.SetHeadBlendData(
            (uint)characterAppearance.FaceFather,
            (uint)characterAppearance.FaceMother,
            0,
            (uint)characterAppearance.SkinFather,
            (uint)characterAppearance.SkinMother,
            0,
            characterAppearance.FaceMix,
            characterAppearance.SkinMix,
            0
        );

        foreach (var characterAppearanceFaceFeature in characterAppearance.FaceFeatures)
        {
            player.SetFaceFeature((byte)characterAppearanceFaceFeature.Index, characterAppearanceFaceFeature.Value);
        }

        foreach (var characterAppearanceFaceOverlay in characterAppearance.FaceOverlays)
        {
            player.SetHeadOverlay(
                characterAppearanceFaceOverlay.Index,
                (byte)characterAppearanceFaceOverlay.Value,
                characterAppearanceFaceOverlay.Opacity
            );

            if (characterAppearanceFaceOverlay.HasColor)
            {
                player.SetHeadOverlayColor(
                    characterAppearanceFaceOverlay.Index,
                    2,
                    characterAppearanceFaceOverlay.FirstColor,
                    characterAppearanceFaceOverlay.FirstColor
                );
            }
        }

        player.SetClothes(2, (ushort)characterAppearance.HairDrawable, 0, 0);
        player.SetHeadOverlay(1, (byte)characterAppearance.FacialHair, characterAppearance.FacialHairOpacity);
        player.SetHeadOverlayColor(
            1,
            1,
            (byte)characterAppearance.FirstFacialHairColor,
            (byte)characterAppearance.SecondFacialHairColor
        );
        player.SetHeadOverlay(2, (byte)characterAppearance.Eyebrows, 1);
        player.SetHeadOverlayColor(
            2,
            1,
            (byte)characterAppearance.EyebrowsColor,
            (byte)characterAppearance.EyebrowsColor
        );
        player.SetEyeColor((ushort)characterAppearance.EyeColor);

        player.HairColor = (byte)characterAppearance.FirstHairColor;
        player.HairHighlightColor = (byte)characterAppearance.SecondHairColor;

        switch (player.Model)
        {
            case (uint)PedModel.FreemodeFemale01:
                player.SetClothes(4, 68, 3, 0);
                player.SetClothes(11, 144, 3, 0);
                player.SetClothes(6, 47, 3, 0);
                player.SetClothes(3, 17, 0, 0);
                player.SetClothes(8, 34, 0, 0);
                break;
            case (uint)PedModel.FreemodeMale01:
                player.SetClothes(4, 66, 3, 0);
                player.SetClothes(11, 147, 3, 0);
                player.SetClothes(6, 46, 3, 0);
                player.SetClothes(3, 165, 16, 0);
                player.SetClothes(8, 15, 0, 0);
                break;
        }

        _ = Task.Run(async () =>
        {
            await Task.Delay(2000).ConfigureAwait(false);
            player.Frozen = false;
        });

        player.Emit("clientNametags:showNametags", true);
    }

    async void CreateCharacter(IPlayer player, string appearanceJson)
    {
        if (!player.Exists || player is not PPlayer protonPlayer)
        {
            return;
        }

        var characterAppearance = JsonSerializer.Deserialize<Character>(appearanceJson, serializerOptions);

        if (characterAppearance == null)
        {
            player.Kick("Invalid appearance. Please try again.");
            return;
        }

        characterAppearance.UserId = protonPlayer.ProtonId;
        await characterHandler.Add(characterAppearance).ConfigureAwait(false);

        SetAppearance(protonPlayer, characterAppearance);
        player.Emit("characterClient:stopCharacterCreator");
    }

    static void StartCharacterCreator(IPlayer player)
    {
        player.Dimension = (int)(int.MaxValue - player.Id);
        player.Frozen = true;

        player.Spawn(new Position(402.90833f, -996.61365f, -99.00013f));
        player.Rotation = new Rotation(0, 0, 3.14f);
        player.Model = (uint)PedModel.FreemodeMale01;
        player.Visible = false;

        player.Emit("characterClient:startCharacterCreator");
    }
}
