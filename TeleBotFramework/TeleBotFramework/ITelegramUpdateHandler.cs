using Telegram.Bot.Types;

namespace TeleBotFramework;
public interface ITelegramUpdateHandler
{
    Task HandleUpdate(Update update);
}