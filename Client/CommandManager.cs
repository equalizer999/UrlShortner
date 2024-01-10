using System.Collections.Immutable;
using System.Text;
using Core.Constants;
using Services;

namespace Client;

/// <summary>
///     Handles prompting the user for input and executing commands using the parsed input.
/// </summary>
public sealed class CommandManager : ICommandManager
{
    private readonly ImmutableList<(CommandAction action, string directive)> _commands;
    private readonly IUrlService _urlService;

    public CommandManager(IUrlService urlService)
    {
        _urlService = urlService;
        _commands = ImmutableList.Create<(CommandAction, string)>(
            (ShortenUrlCommand, "Shorten URL"),
            (ShortenUrlCommandWithCustomCode, "Shorten URL with custom code"),
            (ExpandShortUrlCommand, "Expand short URL"),
            (DeleteShortUrlCommand, "Delete short URL"),
            (DeleteAllShortUrlsByLongUrlCommand, "Delete all short URLs associated to the original URL"),
            (GetClickCountCommand, "Get the click count of a short URL"),
            (ExportCommand, "Export the datastore to a json file"),
            (ImportCommand, "Import the datastore from a json file"),
            (ExitCommand, "Exit")
        );

        CommandsListDirective = BuildCommandsDirective();
    }

    public string CommandsListDirective { get; }

    /// <summary>
    ///     Matches the numerical input key to a command action and executes it.
    /// </summary>
    /// <param name="input"></param>
    public void ExecuteCommand(char input)
    {
        Console.Write(": ");
        var index = input - '0';
        if (index < 1 || index > _commands.Count)
        {
            Console.WriteLine("Invalid command. Press enter to continue.");
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
            return;
        }
        _commands[index - 1].action();
    }

    private void ShortenUrlCommand()
    {
        if (TryGetUserInput("Enter a URL to shorten:", out var input))
        {
            var shortUrl = _urlService.ShortenUrl(input);
            if (!shortUrl.IsSuccess)
            {
                Console.WriteLine(shortUrl.ErrorMessage);
                return;
            }
            Console.WriteLine($"Shortened URL: {shortUrl.Value}");
        }
    }

    private void ShortenUrlCommandWithCustomCode()
    {
        if (TryGetUserInput("Enter a URL to shorten:", out var input))
        {
            if (TryGetUserInput("Enter a custom code:", out var code))
            {
                var shortUrl = _urlService.ShortenUrl(input, code);
                if (!shortUrl.IsSuccess)
                {
                    Console.WriteLine(shortUrl.ErrorMessage);
                    return;
                }
                Console.WriteLine($"Shortened URL: {shortUrl.Value}");
            }
        }
    }

    private void ExpandShortUrlCommand()
    {
        if (TryGetUserInput("Enter a short URL to retrieve the original URL:", out var input))
        {
            var longUrl = _urlService.GetOriginalUrl(input);
            if (!longUrl.IsSuccess)
            {
                Console.WriteLine(longUrl.ErrorMessage);
                return;
            }
            Console.WriteLine($"Expanded URL: {longUrl.Value}");
        }
    }

    private void DeleteShortUrlCommand()
    {
        if (TryGetUserInput("Enter a short URL to delete:", out var input))
        {
            var res = _urlService.DeleteShortUrl(input);
            if (!res.IsSuccess)
            {
                Console.WriteLine(res.ErrorMessage);
                return;
            }
            Console.WriteLine($"Short URL Deleted: {res.Value}");
        }
    }

    private void DeleteAllShortUrlsByLongUrlCommand()
    {
        if (TryGetUserInput("Enter a long URL to delete all associated short URLs:", out var input))
        {
            var res = _urlService.DeleteAllShortUrlsByOriginalUrl(input);
            if (!res.IsSuccess)
            {
                Console.WriteLine(res.ErrorMessage);
                return;
            }
            Console.WriteLine($"All short URLs Deleted: {res.Value}");
        }
    }

    private void GetClickCountCommand()
    {
        if (TryGetUserInput("Enter a short URL to retrieve the number of times it has been clicked:", out var input))
        {
            var res = _urlService.GetClickCount(input);
            if (!res.IsSuccess)
            {
                Console.WriteLine(res.ErrorMessage);
                return;
            }
            Console.Write("Click count: ");
            Console.WriteLine(res.Value);
        }
    }

    private void ExportCommand()
    {
        if (TryGetUserInput("Enter a file path to save the datastore as a json file:", out var input))
        {
            try
            {
                _urlService.ExportDatastore(input);
                Console.WriteLine("Datastore exported successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred while exporting the datastore: {e.Message}");
            }
        }
    }

    private void ImportCommand()
    {
        if (TryGetUserInput("Enter a file path to load the datastore from a json file:", out var input))
        {
            try
            {
                _urlService.ImportDatastore(input);
                Console.WriteLine("Datastore imported successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred while importing the datastore: {e.Message}");
            }
        }
    }

    private static void ExitCommand()
    {
        Console.WriteLine("Exiting...");
        Environment.Exit(0);
    }

    private static bool TryGetUserInput(string directive, out string input)
    {
        Console.WriteLine(directive);
        input = Console.ReadLine() ?? string.Empty;
        input = input.Trim();
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine(Errors.EmptyInput);
            return false;
        }
        return true;
    }

    private string BuildCommandsDirective()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Directives.EnterCommand);
        for (var i = 0; i < _commands.Count; i++)
        {
            sb.AppendLine($"{i + 1}: {_commands[i].directive}");
        }
        return sb.ToString();
    }

    private delegate void CommandAction();
}
