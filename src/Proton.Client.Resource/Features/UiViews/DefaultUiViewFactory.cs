using AltV.Net.Client;
using AltV.Net.Client.Elements.Interfaces;
using Proton.Client.Resource.Features.UiViews.Abstractions;

namespace Proton.Client.Resource.Features.UiViews;

public class DefaultUiViewFactory : IUiViewFactory
{
    public IWebView Create(ICore core, IntPtr baseObjectPointer, uint id)
    {
        return new DefaultUiView(core, baseObjectPointer, id);
    }
}
