using System.Reflection;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Args;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Proton.Shared.Adapters;
using Proton.Shared.Dtos;
using Proton.Server.Resource.Authentication.Extentions;
using Proton.Shared.Interfaces;
using System.Globalization;

namespace Proton.Server.Resource;

public sealed class ServerResource : AsyncResource
{
    private readonly IServiceProvider serviceProvider = null!;
    public ServerResource()
    {
        var serviceCollection = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile("appsettings.Local.json", false, true)
            .AddEnvironmentVariables()
            .Build();

        serviceProvider = serviceCollection
            .AddInfrastructure(configuration)
            .AddRaces()
            .AddSingleton<IConfiguration>(configuration)
            .AddAuthentication()
            .BuildServiceProvider();
    }

    public override void OnStart()
    {
        Alt.RegisterMValueAdapter(SharedRaceCreatorDataMValueAdapter.Instance);
        Alt.RegisterMValueAdapter(RaceMapDto.Adapter.Instance);
        Alt.RegisterMValueAdapter(DefaultMValueAdapters.GetArrayAdapter(RaceMapDto.Adapter.Instance));
        Alt.RegisterMValueAdapter(RaceHostSubmitDto.Adapter.Instance);

        // TODO: Add logging for startup
        var services = serviceProvider.GetServices<IStartup>();
    }

    public override void OnStop() { }
}
