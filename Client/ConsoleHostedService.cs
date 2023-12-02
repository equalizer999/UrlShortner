using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Client;

public sealed class ConsoleHostedService : IHostedService
{
    private readonly ICommandManager _commandManager;
    private readonly ILogger<ConsoleHostedService> _logger;

    public ConsoleHostedService(ILogger<ConsoleHostedService> logger,
        ICommandManager commandManager)
    {
        _logger = logger;
        _commandManager = commandManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Console hosted service is starting");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine(_commandManager.CommandsListDirective);
                    var commandKey = Console.ReadKey();
                    _commandManager.ExecuteCommand(commandKey.KeyChar);
                    Console.WriteLine();
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    Console.Write("Something unexpected happened. ");
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                    Console.WriteLine("Please try again.");
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Console hosted service encountered an error");
            throw;
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Console hosted service is stopping");
        return Task.CompletedTask;
    }
}
