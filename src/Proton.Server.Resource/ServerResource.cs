using System.Reflection;
using AltV.Net.Async;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Proton.Server.Resource.Authentication.Extentions;
using Proton.Shared.Interfaces;
using AltV.Net.Elements.Entities;
using Proton.Server.Infrastructure.Factorys;
using Proton.Shared.Extensions;
using AltV.Net;

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
            .AddSingleton<IConfiguration>(configuration)
            .AddAuthentication()
            .AddRaceFeatures()
            .BuildServiceProvider();
    }

    public override void OnStart()
    {
        ResourceExtensions.RegisterMValueAdapters();

        // TODO: Add logging for startup
        var services = serviceProvider.GetServices<IStartup>();
    }

    public override IEntityFactory<IPlayer> GetPlayerFactory()
    {
        return new PPlayerFactory();
    }

    public override void OnStop() { }
}
