using AltV.Net.Client;
using Proton.Client.Resource.Features.UiViews;
using Proton.Client.Resource.Features.UiViews.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddUiViews(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IUiViewFactory, DefaultUiViewFactory>();
        serviceCollection.AddSingleton(provider => (IUiView)Alt.CreateWebView("http://localhost:5173"));
        return serviceCollection;
    }
}
