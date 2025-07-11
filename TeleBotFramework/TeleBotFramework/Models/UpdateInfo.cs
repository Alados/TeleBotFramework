using Telegram.Bot.Types.ReplyMarkups;

namespace TeleBotFramework.Models;
public class UpdateInfo
{
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public int MessageId { get; set; }
    public string? Text { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public InlineKeyboardMarkup? InlineKeyboardMarkup { get; set; }
}
