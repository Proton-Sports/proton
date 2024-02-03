namespace Proton.Client.Core.Interfaces;

public interface IScriptCameraFactory
{
    IScriptCamera RenderingCamera { get; }

    IScriptCamera CreateScriptCamera();
    IScriptCamera CreateScriptCamera(ICameraHash cameraHash);
    IScriptCamera CreateScriptCamera(ICameraHash cameraHash, bool active);
}
