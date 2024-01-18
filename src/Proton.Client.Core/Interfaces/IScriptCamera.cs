using System.Numerics;

namespace Proton.Client.Core.Interfaces;

public interface IScriptCamera : IDisposable
{
    int Id { get; }
    bool IsActive { get; set; }
    bool IsRendering { get; }
    Vector3 Position { get; set; }
    Vector3 Rotation { get; set; }
    Vector3 ForwardVector { get; }

    void SetActiveWithInterpolation(TimeSpan duration, bool easeLocation, bool easeRotation);
    void SetActiveWithInterpolation(IScriptCamera fromCamera, TimeSpan duration, bool easeLocation, bool easeRotation);
    void Render();
    void Render(TimeSpan easeTime);
    void Unrender();
    void Unrender(TimeSpan easeTime);
}
