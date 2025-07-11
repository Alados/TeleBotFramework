using TeleBotFramework.Commands;
using TeleBotFramework.Models;
using Telegram.Bot;

namespace TeleBotFramework.SimpleExample.Commands;

internal class StartCommand(ITelegramBotClient bot) : ITelegramCommand
{
    private readonly ITelegramBotClient _bot = bot;

    public static string Name => "/start";
    public static string Description => "Start operation";
    public static bool IsPublic => true;

    public async Task Execute(UpdateInfo update)
    {
        await _bot.SendMessage(update.ChatId, $"Hello, {update.Username ?? update.FirstName + " " + update.LastName}");
    }
}

