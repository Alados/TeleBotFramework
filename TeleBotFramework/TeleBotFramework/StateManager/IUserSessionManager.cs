using TeleBotFramework.Models;

namespace TeleBotFramework.StateManager;
public interface IUserSessionManager
{
    void ClearSession(long userId);
    public UserSession? GetSession(long userId);
    public UserSession CreateOrUpdateSession(long userId, string command, int step);
}