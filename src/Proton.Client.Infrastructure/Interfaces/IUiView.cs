using AltV.Net.Client.Elements.Interfaces;
using Proton.Shared.Contants;

namespace Proton.Client.Infrastructure.Interfaces;

public interface IUiView : IWebView
{
    void Mount(Route route);
    void Unmount(Route route);
}
