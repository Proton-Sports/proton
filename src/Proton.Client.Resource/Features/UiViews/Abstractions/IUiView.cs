using AltV.Net.Client.Elements.Interfaces;
using Proton.Shared.Contants;

namespace Proton.Client.Resource.Features.UiViews.Abstractions;

public interface IUiView : IWebView
{
    // Called before a route is mounted, can be cancelled
    event Action<Route, MountingEventArgs> Mounting;

    void Mount(Route route);
    Task<bool> TryMountAsync(Route route);
    void Unmount(Route route);
    bool IsMounted(Route route);
    Action OnMount(Route route, Action callback);
    Action OnUnmount(Route route, Action callback);
}
