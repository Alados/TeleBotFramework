using TeleBotFramework.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleBotFramework.Commands;
internal class HelpCommand(ICommandFactory commandFactory, ITelegramBotClient bot) : ITelegramCommand
{
    private readonly ICommandFactory _commandFactory = commandFactory;
    private readonly ITelegramBotClient _bot = bot;

    public static string Name => "/help";
    public static string Description => "Shows command list";
    public static bool IsPublic => true;

    public async Task Execute(UpdateInfo update)
    {
        var commands = _commandFactory.GetCommandList();
        var commandList = string.Join("\n", commands.Select(c => $"{c.Name} - {c.Description}"));
        await _bot.SendMessage(update.ChatId, commandList);
    }
}
