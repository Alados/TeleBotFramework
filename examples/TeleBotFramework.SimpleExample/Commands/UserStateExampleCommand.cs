using TeleBotFramework.Commands;
using TeleBotFramework.Models;
using TeleBotFramework.StateManager;
using Telegram.Bot;

namespace TeleBotFramework.SimpleExample.Commands;

internal class UserStateExampleCommand(ITelegramBotClient bot, IUserSessionManager userSessionManager) : ITelegramCommand
{
    private readonly ITelegramBotClient _bot = bot;
    private readonly IUserSessionManager _userSessionManager = userSessionManager;

    public static string Name => "/resend";
    public static string Description => "Resend message to chat";
    public static bool IsPublic => true;

    public async Task Execute(UpdateInfo update)
    {
        var currentSession = _userSessionManager.GetSession(update.UserId);
        var currentStep  = currentSession?.Step ?? 0;
        switch (currentStep)
        {
            case 0:
                await _bot.SendMessage(update.ChatId, "Hi, enter message and I'll resend it to you");
                _userSessionManager.CreateOrUpdateSession(update.UserId, Name, 1, ["arg"]);
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