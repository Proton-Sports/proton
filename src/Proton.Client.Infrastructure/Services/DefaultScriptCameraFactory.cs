using AltV.Net.Client;
using Proton.Client.Core.Interfaces;
using Proton.Client.Infrastructure.Constants;

namespace Proton.Client.Infrastructure.Services;

public class DefaultScriptCameraFactory : IScriptCameraFactory
{
    public IScriptCamera RenderingCamera => DefaultScriptCamera.From(Alt.Natives.GetRenderingCam());

    public IScriptCamera CreateScriptCamera() => CreateScriptCamera(CameraHash.Scripted, false);
    public IScriptCamera CreateScriptCamera(ICameraHash cameraHash) => CreateScriptCamera(cameraHash, false);
    public IScriptCamera CreateScriptCamera(ICameraHash cameraHash, bool active) => new DefaultScriptCamera(Alt.Natives.CreateCamera(cameraHash.Hash, active));
}
