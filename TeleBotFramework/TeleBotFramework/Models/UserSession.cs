namespace TeleBotFramework.Models;
public class UserSession
{
    public UserSession(long telegramUserId, string command, int step, string[]? args)
    {
        TelegramUserId = telegramUserId;
        Command = command;
        Step = step;
        Arguments = args ?? [];
    }

    public long TelegramUserId { get; set; }
    public string Command { get; set; }
    public int Step { get; set; }
    public string[] Arguments { get; set; } = [];
}
