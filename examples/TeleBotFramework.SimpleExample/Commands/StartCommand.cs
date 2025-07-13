using TeleBotFramework.Attributes;
using TeleBotFramework.Commands;
using TeleBotFramework.Models;
using Telegram.Bot;

namespace TeleBotFramework.SimpleExample.Commands;

[Command("/start", "Start operation", true)]
internal class StartCommand(ITelegramBotClient bot) : ITelegramCommand
{
    private readonly ITelegramBotClient _bot = bot;

    public async Task Execute(UpdateInfo update)
    {
        await _bot.SendMessage(update.ChatId, $"Hello, {update.Username ?? update.FirstName + " " + update.LastName}");
    }
}

