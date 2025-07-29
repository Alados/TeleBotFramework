using TeleBotFramework.Attributes;
using TeleBotFramework.Client;
using TeleBotFramework.Commands;
using TeleBotFramework.Models;
using TeleBotFramework.StateManager;

namespace TeleBotFramework.SimpleExample.Commands;

[Command(_commandName, "Resend message to chat", true)]
internal class UserStateExampleCommand(ITeleBotClient bot, IUserSessionManager userSessionManager) : ITelegramCommand
{
    private readonly ITeleBotClient _bot = bot;
    private readonly IUserSessionManager _userSessionManager = userSessionManager;
    private const string _commandName = "/resend";

    public async Task Execute(UpdateInfo update)
    {
        var currentSession = _userSessionManager.GetSession(update.UserId);
        var currentStep  = currentSession?.Step ?? 0;
        switch (currentStep)
        {
            case 0:
                await _bot.SendMessage(update.ChatId, "Hi, enter message and I'll resend it to you");
                _userSessionManager.CreateOrUpdateSession(update.UserId, _commandName, 1, ["arg"]);
                break;
            case 1:
                if (string.IsNullOrWhiteSpace(update.Text))
                {
                    await _bot.SendMessage(update.ChatId, "Please enter a valid message.");
                    return;
                }
                await _bot.SendMessage(update.ChatId, $"You entered: {update.Text}. Argument was {currentSession!.Arguments[0]}");
                _userSessionManager.ClearSession(update.UserId);
                break;
        }
    }
}