using Telegram.Bot.Types.ReplyMarkups;

namespace TeleBotFramework.Client;
public interface ITeleBotClient
{
    Task DeleteMessage(long chatId, int messageId);
    Task EditMessageReplyMarkup(long chatId, int messageId, InlineKeyboardMarkup? replyMarkup = null);
    Task SendMessage(long chatId, string message, InlineKeyboardMarkup? replyMarkup = null);
}