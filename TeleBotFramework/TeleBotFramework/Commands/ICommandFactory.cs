using TeleBotFramework.Models;

namespace TeleBotFramework.Commands;

public interface ICommandFactory
{
    ITelegramCommand? CreateCommand(string commandName);
    IList<CommandInfo> GetCommandList();
}