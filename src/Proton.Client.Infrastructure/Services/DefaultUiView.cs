using AltV.Net.Client;
using AltV.Net.Client.Elements.Entities;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Shared.Contants;

namespace Proton.Client.Infrastructure.Services;

public class DefaultUiView : WebView, IUiView
{
    public DefaultUiView(ICore core, IntPtr webViewNativePointer, uint id) : base(core, webViewNativePointer, id) { }

    public void Mount(Route route)
    {
        Emit("webview.mount", route.Value);
    }

    public void Unmount(Route route)
    {
        Emit("webview.unmount", route.Value);
    }
}
