using System.Reflection;
using AltV.Net.Async;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            .BuildServiceProvider();
    }

    public override void OnStart() { }

    public override void OnStop() { }
}
