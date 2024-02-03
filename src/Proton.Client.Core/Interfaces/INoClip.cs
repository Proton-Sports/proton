using Proton.Client.Core.Models;

namespace Proton.Client.Core.Interfaces;

public interface INoClip
{
    bool IsStarted { get; }
    bool TryGetRaycastData(out RaycastData data);
    void Start();
    void Stop();
}
