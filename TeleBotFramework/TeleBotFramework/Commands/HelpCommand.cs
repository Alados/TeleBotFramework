using TeleBotFramework.Attributes;
using TeleBotFramework.Models;
using Telegram.Bot;

namespace TeleBotFramework.Commands;

[Command("/help", "Shows command list", true)]
internal class HelpCommand(ICommandFactory commandFactory, ITelegramBotClient bot) : ITelegramCommand
{
    private readonly ICommandFactory _commandFactory = commandFactory;
    private readonly ITelegramBotClient _bot = bot;

    public async Task Execute(UpdateInfo update)
    {
        var commands = _commandFactory.GetCommandList();
        var commandList = string.Join("\n", commands.Select(c => $"{c.Name} - {c.Description}"));
        await _bot.SendMessage(update.ChatId, commandList);
    }
}
