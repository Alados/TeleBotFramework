using TeleBotFramework.Models;
using TeleBotFramework.StateManager;

namespace TeleBotFramework.Commands;
internal class CancelCommand(IUserSessionManager sessionManager) : ITelegramCommand
{
    private readonly IUserSessionManager _sessionManager = sessionManager;
    public static string Name => "/cancel";
    public static string Description => "Cancel the current operation";
    public static bool IsPublic => true;

    public async Task Execute(UpdateInfo update)
    {
        _sessionManager.ClearSession(update.UserId);
    }
}
