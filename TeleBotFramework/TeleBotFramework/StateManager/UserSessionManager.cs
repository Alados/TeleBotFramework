using System.Collections.Concurrent;
using TeleBotFramework.Models;

namespace TeleBotFramework.StateManager;

public class UserSessionManager : IUserSessionManager
{
    private readonly ConcurrentDictionary<long, UserSession> _sessions = new();

    public UserSession? GetSession(long userId)
    {
        return _sessions.GetValueOrDefault(userId);
    }

    public UserSession CreateOrUpdateSession(long userId, string command, int step, string[]? args = null)
    {
        return _sessions.AddOrUpdate(userId, _ => new UserSession(userId, command, step, args), (key, oldValue) =>
        {
            oldValue.Command = command;
            oldValue.Step = step;
            oldValue.Arguments = args ?? oldValue.Arguments;
            return oldValue;
        });
    }

    public void ClearSession(long userId)
    {
        _sessions.TryRemove(userId, out _);
    }
}
