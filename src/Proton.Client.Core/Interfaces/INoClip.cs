using Proton.Client.Core.Models;

namespace Proton.Client.Core.Interfaces;

public interface INoClip
{
    IScriptCamera? Camera { get; }
    bool IsStarted { get; }
    bool TryGetRaycastData(out RaycastData data);
    void Start();
    void Stop();
}
