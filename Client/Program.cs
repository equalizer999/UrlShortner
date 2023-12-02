using Client;
using Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices(static services =>
    {
        services.AddSingleton<IUrlDatastore, UrlDatastore>();
        services.AddSingleton<IUrlService, UrlService>();
        services.AddSingleton<ICommandManager, CommandManager>();
        services.AddHostedService<ConsoleHostedService>();
    })
    .RunConsoleAsync();
