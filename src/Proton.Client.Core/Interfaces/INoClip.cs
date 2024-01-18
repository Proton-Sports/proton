namespace Proton.Client.Core.Interfaces;

public interface INoClip
{
    bool IsStarted { get; }
    void Start();
    void Stop();
}
