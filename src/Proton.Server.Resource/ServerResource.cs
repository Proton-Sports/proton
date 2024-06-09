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
using Proton.Server.Resource.CharacterCreator.Extensions;
using Microsoft.Extensions.Hosting;

namespace Proton.Server.Resource;

public sealed class ServerResource : AsyncResource
{
    private readonly IHost host;

    public ServerResource()
    {
        var builder = Host.CreateDefaultBuilder();

        builder.UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);
        builder.ConfigureAppConfiguration((builder) =>
        {
            builder.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);
        });
        builder.ConfigureServices((context, builder) =>
        {
            builder
                .AddSingleton<IHostLifetime, ResourceLifetime>()
                .AddInfrastructure(context.Configuration)
                .AddAuthentication()
                .AddRaceFeatures()
                .AddCharacterCreator();
        });

        host = builder.Build();
    }

    public override void OnStart()
    {
        ResourceExtensions.RegisterMValueAdapters();

        // TODO: Add logging for startup
        _ = host.Services.GetServices<IStartup>();
        host.StartAsync().Wait();
    }

    public override void OnStop()
    {
        host.StopAsync().Wait();
        host.Dispose();
    }

    public override IEntityFactory<IPlayer> GetPlayerFactory()
    {
        return new PPlayerFactory();
    }

    private class ResourceLifetime : IHostLifetime
    {
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
