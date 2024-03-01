using AltV.Net.Client.Elements.Interfaces;
using Proton.Shared.Contants;

namespace Proton.Client.Infrastructure.Interfaces;

public interface IUiView : IWebView
{
    void Mount(Route route);
    Task<bool> TryMountAsync(Route route);
    void Unmount(Route route);
    bool IsMounted(Route route);
}
