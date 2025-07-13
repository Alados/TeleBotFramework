using TeleBotFramework.Attributes;
using TeleBotFramework.Models;
using TeleBotFramework.StateManager;

namespace TeleBotFramework.Commands;
[Command("/cancel", "Cancel the current operation", true)]
internal class CancelCommand(IUserSessionManager sessionManager) : ITelegramCommand
{
    private readonly IUserSessionManager _sessionManager = sessionManager;
    public async Task Execute(UpdateInfo update)
    {
        _sessionManager.ClearSession(update.UserId);
    }
}
