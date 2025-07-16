using TeleBotFramework.Attributes;
using TeleBotFramework.Client;
using TeleBotFramework.Commands;
using TeleBotFramework.Models;

namespace TeleBotFramework.SimpleExample.Commands;

[Command("/start", "Start operation", true)]
internal class StartCommand(ITeleBotClient bot) : ITelegramCommand
{
    private readonly ITeleBotClient _bot = bot;

    public async Task Execute(UpdateInfo update)
    {
        await _bot.SendMessage(update.ChatId, $"Hello, {update.Username ?? update.FirstName + " " + update.LastName}");
    }
}

