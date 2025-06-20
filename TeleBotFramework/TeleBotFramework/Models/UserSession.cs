namespace TeleBotFramework.Models;
public class UserSession
{
    public UserSession(long telegramUserId, string command, int step)
    {
        TelegramUserId = telegramUserId;
        Command = command;
        Step = step;
    }

    public long TelegramUserId { get; set; }
    public string Command { get; set; }
    public int Step { get; set; }
}
