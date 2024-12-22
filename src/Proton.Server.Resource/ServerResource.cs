using System.Reflection;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.Authentication.Extentions;
using Proton.Server.Resource.CharacterCreator.Extensions;
using Proton.Server.Resource.Features.Shop;
<<<<<<< HEAD
using Proton.Server.Resource.Features.Vehicles;
=======
>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1
using Proton.Shared.Extensions;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource;

public sealed class ServerResource : AsyncResource
{
    private readonly IHost host;

    public ServerResource()
    {
        var builder = Host.CreateDefaultBuilder();

        builder.UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);
        builder.ConfigureAppConfiguration(
            (builder) =>
            {
                builder.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);
            }
        );
        builder.ConfigureServices(
            (context, builder) =>
            {
                builder
                    .AddMemoryCache()
                    .AddSingleton<IHostLifetime, ResourceLifetime>()
                    .AddInfrastructure()
                    .AddAuthentication()
                    .AddPlayerFeatures()
                    .AddRaceFeatures()
                    .AddTuningFeature()
                    .AddCharacterCreator()
                    .AddIplFeatures()
<<<<<<< HEAD
                    .AddVehicleFeatures()
=======
>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1
                    .AddShops();
            }
        );

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

    public override IEntityFactory<IVehicle> GetVehicleFactory()
    {
        return new ProtonVehicleFactory();
    }

    private sealed class ResourceLifetime : IHostLifetime
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
