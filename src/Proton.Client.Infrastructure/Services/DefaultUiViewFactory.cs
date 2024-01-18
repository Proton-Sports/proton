using AltV.Net.Client;
using AltV.Net.Client.Elements.Interfaces;
using Proton.Client.Infrastructure.Interfaces;

namespace Proton.Client.Infrastructure.Services;

public class DefaultUiViewFactory : IUiViewFactory
{
    public IWebView Create(ICore core, IntPtr baseObjectPointer, uint id)
    {
        return new DefaultUiView(core, baseObjectPointer, id);
    }
}
