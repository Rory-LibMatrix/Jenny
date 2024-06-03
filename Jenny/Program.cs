using Jenny;
using Jenny.Handlers;
using LibMatrix.Services;
using LibMatrix.Utilities.Bot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureHostOptions(host => {
    host.ServicesStartConcurrently = true;
    host.ServicesStopConcurrently = true;
    host.ShutdownTimeout = TimeSpan.FromSeconds(5);
});

if (Environment.GetEnvironmentVariable("JENNY_APPSETTINGS_PATH") is string path)
    builder.ConfigureAppConfiguration(x => x.AddJsonFile(path));

var host = builder.ConfigureServices((_, services) => {
    services.AddSingleton<JennyConfiguration>();

    services.AddRoryLibMatrixServices(new() {
        AppName = "Jenny"
    });
    services.AddMatrixBot().AddCommandHandler().DiscoverAllCommands()
        .WithInviteHandler(InviteHandler.HandleAsync)
        .WithCommandResultHandler(CommandResultHandler.HandleAsync);

    services.AddHostedService<JennyBot>();
}).UseConsoleLifetime().Build();

await host.RunAsync();