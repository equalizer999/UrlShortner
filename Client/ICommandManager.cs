namespace Client;

public interface ICommandManager
{
    string CommandsListDirective { get; }
    void ExecuteCommand(char input);
}
