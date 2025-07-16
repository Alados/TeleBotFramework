using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleBotFramework.Client;
public class TeleBotClient(ITelegramBotClient botClient) : ITeleBotClient
{
    private readonly ITelegramBotClient _botClient = botClient;
    private int _messagesSentThisSecond = 0;
    private DateTime _lastResetTime = DateTime.UtcNow;

    public async Task SendMessage(long chatId, string message, InlineKeyboardMarkup? replyMarkup = null)
    {
        await SendMessageWithRateLimit(_botClient.SendMessage(chatId, message, replyMarkup: replyMarkup), 20);
    }

    public async Task EditMessageReplyMarkup(long chatId, int messageId, InlineKeyboardMarkup? replyMarkup = null)
    {
        await SendMessageWithRateLimit(_botClient.EditMessageReplyMarkup(chatId, messageId, replyMarkup: replyMarkup), 20);
    }

    public async Task DeleteMessage(long chatId, int messageId)
    {
        await SendMessageWithRateLimit(_botClient.DeleteMessage(chatId, messageId), 20);
    }

    private async Task SendMessageWithRateLimit(Task sendMessage, int maxRequestPerSecond = 20)
    {
        lock (this)
        {
            var now = DateTime.UtcNow;
            if ((now - _lastResetTime).TotalSeconds >= 1)
            {
                _messagesSentThisSecond = 0;
                _lastResetTime = now;
            }

            if (_messagesSentThisSecond >= maxRequestPerSecond)
            {
                var waitTime = 1000 - (int)(now - _lastResetTime).TotalMilliseconds;
                if (waitTime > 0)
                    Thread.Sleep(waitTime);

                _messagesSentThisSecond = 0;
                _lastResetTime = DateTime.UtcNow;
            }

            _messagesSentThisSecond++;
        }

        try
        {
            await sendMessage;
        }
        catch (ApiRequestException ex) when (ex.ErrorCode == 429)
        {
            var retryAfter = ex.Parameters?.RetryAfter ?? 1;
            await Task.Delay(retryAfter * 1000);
        }
    }
}
