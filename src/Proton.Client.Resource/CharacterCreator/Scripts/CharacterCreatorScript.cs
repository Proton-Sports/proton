using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using AltV.Net.Client;
using AltV.Net.Client.Async;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;
using AltV.Net.Enums;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Server.Resource.CharacterCreator.Models;
using Proton.Shared.Contants;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.CharacterCreator.Scripts;

public class CharacterCreatorScript : IStartup
{
    private readonly IUiView uiView;
    private int characterCamera;
    private int cameraFov = 70;
    private uint controlsInterval;
    private float zPos = 0.5f;
    private bool mouseEntered;
    private int locationInterior;
    private Position startCameraPosition;
    private Position startPosition;
    private readonly Position locationPosition;
    private ILocalPed? characterPed;
    
    public CharacterCreatorScript(IUiView uiView)
    {
        locationPosition = new Position(402.90833f, -996.61365f, -99.00013f);
        this.uiView = uiView;
        
        Alt.OnServer("characterClient:startCharacterCreator", StartCharacterCreator);
        Alt.OnServer("characterClient:stopCharacterCreator", StopCharacterCreator);
        Alt.OnServer<uint>("characterClient:createCharacter", CreateCharacter);
        
        this.uiView.On<int>("characterClient:setGender", SetGender);
        this.uiView.On<string>("characterClient:setAppearance", SetAppearance);
        this.uiView.On<string>("characterClient:submitAppearance", SubmitAppearance);
        this.uiView.On<bool>("characterClient:mouseEntered", MouseEntered);

        this.uiView.OnMount(Route.CharacterCreator, () =>
        {
            uiView.Focus();
        
            Alt.Natives.DoScreenFadeIn(1000);
            Alt.ShowCursor(true);    
        });
    }

    private void MouseEntered(bool isMouseEntered)
    {
        Alt.Log("aici " + isMouseEntered);
        mouseEntered = isMouseEntered;
    }

    private async void StopCharacterCreator()
    {
        uiView.Unmount(Route.CharacterCreator);
        
        Alt.Natives.DoScreenFadeOut(1000);
        uiView.Unfocus();

        await AltAsync.WaitFor(() => Alt.Natives.IsScreenFadedOut(), timeout: 10000);
        
        DeleteCharacter();
        DeleteCharacterCreatorCamera();

        Alt.FocusData.ClearFocusOverride();
        Alt.Natives.DoScreenFadeIn(1000);
        Alt.Natives.UnpinInterior(locationInterior);
        
        Alt.ShowCursor(false);
    }
    
    private void DeleteCharacter()
    {
        if (characterPed == null) return;
        if (!characterPed.Exists) return;
        
        characterPed.Destroy();
    }
    
    private void DeleteCharacterCreatorCamera()
    {
        var doesCameraExists = Alt.Natives.DoesCamExist(characterCamera);
        if (!doesCameraExists) return;
        
        Alt.Natives.SetCamActive(characterCamera, false);
        Alt.Natives.RenderScriptCams(false, false,0, true, false, 0);
        Alt.Natives.DestroyCam(characterCamera, true);
        
        characterCamera = 0;
        
        Alt.ClearInterval(controlsInterval);
    }
    
    private void SubmitAppearance(string appearanceJson)
    {
        Alt.EmitServer("characterServer:setAppearance", appearanceJson);
    }
    
    private void SetGender(int selectedModel)
    {
        if (characterPed == null) return;
        
        var doesEntityExists = characterPed.Exists;
        if (!doesEntityExists) return;
        
        uint selectedModelUint = selectedModel switch
        {
            1 => (uint) PedModel.FreemodeMale01,
            0 => (uint) PedModel.FreemodeFemale01,
            _ => 0
        };

        var currentModel = characterPed.Model;
        if (currentModel == selectedModelUint) return;

        CreateCharacter(selectedModelUint);
    }
    
    private void SetCharacterClothes(uint characterModel)
    {
        if (characterPed == null) return;
        
        var doesEntityExist = characterPed.Exists;
        if (!doesEntityExist) return;
        
        switch (characterModel)
        {
            case (uint)PedModel.FreemodeFemale01:
                Alt.Natives.SetPedComponentVariation(characterPed.ScriptId, 4, 68, 3, 0);
                Alt.Natives.SetPedComponentVariation(characterPed.ScriptId, 11, 144, 3, 0);
                Alt.Natives.SetPedComponentVariation(characterPed.ScriptId, 6, 47, 3, 0);
                Alt.Natives.SetPedComponentVariation(characterPed.ScriptId, 3, 17, 0, 0);
                Alt.Natives.SetPedComponentVariation(characterPed.ScriptId, 8, 34, 0, 0);
                break;
            case (uint)PedModel.FreemodeMale01: 
                Alt.Natives.SetPedComponentVariation(characterPed.ScriptId, 4, 66, 3, 0);
                Alt.Natives.SetPedComponentVariation(characterPed.ScriptId, 11, 147, 3, 0);
                Alt.Natives.SetPedComponentVariation(characterPed.ScriptId, 6, 46, 3, 0);
                Alt.Natives.SetPedComponentVariation(characterPed.ScriptId, 3, 165, 16, 0);
                Alt.Natives.SetPedComponentVariation(characterPed.ScriptId, 8, 15, 0, 0);
                break;
        }
    }
    
    private void SetAppearance(string appearanceJson)
    {
        if (characterPed == null) return;
        
        var doesEntityExist = characterPed.Exists;
        if (!doesEntityExist) return;

        var characterAppearance = JsonSerializer.Deserialize<Character>(appearanceJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        });

        if (characterAppearance == null)
        {
            Alt.LogError("Invalid appearance.");
            return;
        }
        
        Alt.Natives.SetPedHeadBlendData(characterPed.ScriptId, 0, 0, 0, 0, 0, 0, 0, 0, 0, false);
        Alt.Natives.SetPedHeadBlendData(characterPed.ScriptId, characterAppearance.FaceFather, characterAppearance.FaceMother,
            0, characterAppearance.SkinFather, characterAppearance.SkinMother, 0, 
            characterAppearance.FaceMix, characterAppearance.SkinMix, 0, false);
        
        foreach (var characterAppearanceFaceFeature in characterAppearance.FaceFeatures)
        {
            Alt.Natives.SetPedMicroMorph(characterPed.ScriptId, characterAppearanceFaceFeature.Index, characterAppearanceFaceFeature.Value);
        }
        
        foreach (var characterAppearanceFaceOverlay in characterAppearance.FaceOverlays)
        {
            Alt.Natives.SetPedHeadOverlay(characterPed.ScriptId, characterAppearanceFaceOverlay.Index, characterAppearanceFaceOverlay.Value, characterAppearanceFaceOverlay.Opacity);

            if (characterAppearanceFaceOverlay.HasColor)
            {
                Alt.Natives.SetPedHeadOverlayTint(characterPed.ScriptId, characterAppearanceFaceOverlay.Index, 2, characterAppearanceFaceOverlay.FirstColor, characterAppearanceFaceOverlay.FirstColor);
            }
        }
        
        Alt.Natives.SetPedComponentVariation(characterPed.ScriptId, 2, characterAppearance.HairDrawable, 0, 0);
        Alt.Natives.SetPedHairTint(characterPed.ScriptId, characterAppearance.FirstHairColor, characterAppearance.SecondHairColor);
        Alt.Natives.SetPedHeadOverlay(characterPed.ScriptId, 1, characterAppearance.FacialHair, characterAppearance.FacialHairOpacity);
        Alt.Natives.SetPedHeadOverlayTint(characterPed.ScriptId, 1, 1, characterAppearance.FirstFacialHairColor, characterAppearance.SecondFacialHairColor);
        Alt.Natives.SetPedHeadOverlay(characterPed.ScriptId, 2, characterAppearance.Eyebrows, 1);
        Alt.Natives.SetPedHeadOverlayTint(characterPed.ScriptId, 2, 1, characterAppearance.EyebrowsColor, characterAppearance.EyebrowsColor);
        Alt.Natives.SetHeadBlendEyeColor(characterPed.ScriptId, characterAppearance.EyeColor);
    }
    
    private void StartCharacterCreator()
    {
        locationInterior = Alt.Natives.GetInteriorAtCoords(locationPosition.X, locationPosition.Y, locationPosition.Z);
        
        Alt.Natives.PinInteriorInMemory(locationInterior);
        Alt.FocusData.OverrideFocusPosition(locationPosition, Vector3.Zero);
        
        var currentModel = Alt.LocalPlayer.Model;
        
        CreateCharacter(currentModel);
        CreateCharacterCreatorCamera();
        SetCharacterClothes(currentModel);

        uiView.Mount(Route.CharacterCreator);
    }
    
    private async void CreateCharacter(uint characterModel)
    {
        if (characterPed is { Exists: true })
        {
            characterPed.Model = characterModel;
            SetCharacterClothes(characterModel);
            
            Alt.Natives.TaskPlayAnim(characterPed.ScriptId, 
                "anim@heists@heist_corona@single_team", 
                "single_team_loop_boss", 
                1, 
                1, 
                -1, 
                1, 
                0, 
                false, false, false);
            
            return;
        } 
        
        characterPed = Alt.CreateLocalPed(characterModel, Alt.LocalPlayer.Dimension, locationPosition, new Rotation(0, 0, (float) 3.14), false, 0);
        characterPed.Rotation = new Rotation(0, 0, (float) 3.14);
        characterPed.Frozen = true;
        
        Alt.Natives.RequestAnimDict("anim@heists@heist_corona@single_team");
        await AltAsync.WaitFor(() => Alt.Natives.HasAnimDictLoaded("anim@heists@heist_corona@single_team") && characterPed.Spawned);
        
        Alt.Natives.TaskPlayAnim(characterPed.ScriptId, 
            "anim@heists@heist_corona@single_team", 
            "single_team_loop_boss", 
            1, 
            1, 
            -1, 
            1, 
            0, 
            false, false, false);
        
        SetCharacterClothes(characterModel);
    }
    
    private void CreateCharacterCreatorCamera()
    {
        startPosition = locationPosition;
        
        var doesCameraExists = Alt.Natives.DoesCamExist(characterCamera);
        if (doesCameraExists) return;

        var forwardVector = Alt.Natives.GetEntityForwardVector(Alt.LocalPlayer);
        var forwardCameraPosition = new Position((float)(startPosition.X + forwardVector.X * 1.2),
            (float)(startPosition.Y + forwardVector.Y * 1.2), startPosition.Z + zPos);

        startCameraPosition = forwardCameraPosition;
        characterCamera = Alt.Natives.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", forwardCameraPosition.X,
            forwardCameraPosition.Y, forwardCameraPosition.Z, 0, 0, 0, cameraFov, true, 0);
        
        Alt.Natives.PointCamAtCoord(characterCamera, startPosition.X, startPosition.Y, startPosition.Z + zPos);
        Alt.Natives.SetCamActive(characterCamera, true);
        Alt.Natives.RenderScriptCams(true, false,0, true, false, 0);

        controlsInterval = Alt.SetInterval(CameraControls, 0); 
    }
    
    private void CameraControls()
    {
        if (characterPed == null) return;
        
        var doesEntityExist = characterPed.Exists;
        if (!doesEntityExist) return;
        
        Alt.Natives.DisableControlAction(0, 0, true);
        Alt.Natives.DisableControlAction(0, 1, true);
        Alt.Natives.DisableControlAction(0, 2, true);
        Alt.Natives.DisableControlAction(0, 16, true);
        Alt.Natives.DisableControlAction(0, 17, true);
        Alt.Natives.DisableControlAction(0, 24, true);
        Alt.Natives.DisableControlAction(0, 25, true);
        Alt.Natives.DisableControlAction(0, 32, true);
        Alt.Natives.DisableControlAction(0, 33, true);
        Alt.Natives.DisableControlAction(0, 34, true);
        Alt.Natives.DisableControlAction(0, 35, true);

        var screenResolution = Alt.ScreenResolution;
        var cursorPosition = Alt.GetCursorPos(false);
        var cursorX = cursorPosition.X;
        var oldHeading = characterPed.Rotation.Yaw;
        
        if (Alt.Natives.IsDisabledControlPressed(0, 17))
        {
            if (!mouseEntered)
            {
                cameraFov -= 2;

                if (cameraFov < 10)
                {
                    cameraFov = 10;
                }

                Alt.Natives.SetCamFov(characterCamera, cameraFov);
                Alt.Natives.SetCamActive(characterCamera, true);
                Alt.Natives.RenderScriptCams(true, false, 0, true, false, 0);
            }
        }

        if (Alt.Natives.IsDisabledControlPressed(0, 16))
        {
            if (!mouseEntered)
            {
                cameraFov += 2;

                if (cameraFov > 130)
                {
                    cameraFov = 130;
                }

                Alt.Natives.SetCamFov(characterCamera, cameraFov);
                Alt.Natives.SetCamActive(characterCamera, true);
                Alt.Natives.RenderScriptCams(true, false, 0, true, false, 0);
            }
        }

        if (Alt.Natives.IsDisabledControlPressed(0, 32))
        {
            zPos += 0.01f;

            if (zPos > 1.2f)
            {
                zPos = 1.2f;
            }

            Alt.Natives.SetCamCoord(characterCamera, startCameraPosition.X, startCameraPosition.Y, startPosition.Z + zPos);
            Alt.Natives.PointCamAtCoord(characterCamera, startPosition.X, startPosition.Y, startPosition.Z + zPos);
            Alt.Natives.SetCamActive(characterCamera, true);
            Alt.Natives.RenderScriptCams(true, false, 0, true, false, 0);
        }

        if (Alt.Natives.IsDisabledControlPressed(0, 33))
        {
            zPos -= 0.01f;

            if (zPos < -1.2f)
            {
                zPos = -1.2f;
            }

            Alt.Natives.SetCamCoord(characterCamera, startCameraPosition.X, startCameraPosition.Y, startPosition.Z + zPos);
            Alt.Natives.PointCamAtCoord(characterCamera, startPosition.X, startPosition.Y, startPosition.Z + zPos);
            Alt.Natives.SetCamActive(characterCamera, true);
            Alt.Natives.RenderScriptCams(true, false, 0, true, false, 0);
        }

        if (Alt.Natives.IsDisabledControlPressed(0, 25))
        {
            if (cursorX < screenResolution.X / 2)
            {
                const double degToRadians = 2 * (Math.PI / 180);
                oldHeading -= (float) degToRadians;
                characterPed.Rotation =
                    new Rotation(characterPed.Rotation.Roll, characterPed.Rotation.Pitch, oldHeading);
            }

            if (cursorX > screenResolution.X / 2)
            {
                const double degToRadians = 2 * (Math.PI / 180);
                oldHeading += (float) degToRadians;
                characterPed.Rotation =
                    new Rotation(characterPed.Rotation.Roll, characterPed.Rotation.Pitch, oldHeading);
            }
        }

        if (Alt.Natives.IsDisabledControlPressed(0, 35))
        {
            const double degToRadians = 2 * (Math.PI / 180);
            oldHeading += (float) degToRadians;
            characterPed.Rotation =
                new Rotation(characterPed.Rotation.Roll, characterPed.Rotation.Pitch, oldHeading);
        }

        if (Alt.Natives.IsDisabledControlPressed(0, 34))
        {
            const double degToRadians = 2 * (Math.PI / 180);
            oldHeading -= (float) degToRadians;
            characterPed.Rotation =
                new Rotation(characterPed.Rotation.Roll, characterPed.Rotation.Pitch, oldHeading);
        }
    }
}