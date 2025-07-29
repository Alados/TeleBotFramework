using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleBotFramework.Client;
public interface ITeleBotClient
{
    Task SendMessage(long chatId, string message, ParseMode parseMode = ParseMode.None, InlineKeyboardMarkup? replyMarkup = null);
    Task EditMessageText(long chatId, int messageId, string newText, InlineKeyboardMarkup? replyMarkup = null);
    Task EditMessageReplyMarkup(long chatId, int messageId, InlineKeyboardMarkup? replyMarkup = null);
    Task DeleteMessage(long chatId, int messageId);
}