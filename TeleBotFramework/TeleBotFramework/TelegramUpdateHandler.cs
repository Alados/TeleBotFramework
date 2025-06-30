using TeleBotFramework.Commands;
using TeleBotFramework.StateManager;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleBotFramework;
public class TelegramUpdateHandler : ITelegramUpdateHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly ICommandFactory _commandFactory;
    private readonly IUserSessionManager _userSessionManager;

    public TelegramUpdateHandler(ICommandFactory commandFactory, ITelegramBotClient bot, IUserSessionManager userSessionManager)
    {
        _commandFactory = commandFactory;
        _bot = bot;
        _userSessionManager = userSessionManager;
    }

    public async Task HandleUpdate(Update update)
    {

        if (update.Message != null)
        {
            var userId = update.Message.From!.Id;
            var userSession = _userSessionManager.GetSession(userId);
            string commandName;
            if (userSession is null)
            {
                if (string.IsNullOrWhiteSpace(update.Message.Text))
                {
                    await _bot.SendMessage(update.Message.Chat.Id, $"Enter command, please");
                    return;
                }

                var commandWithParams = update.Message.Text.Split(' ');
                commandName = commandWithParams[0];
            }
            else
            {
                commandName = userSession.Command;
            }

            var command = _commandFactory.CreateCommand(commandName);
            if (command is null)
            {
                await _bot.SendMessage(update.Message.Chat.Id, $"Unknown command: {commandName}");
                return;
            }

            await command.Execute(update);
        }

        if (update.CallbackQuery is { } cb)
        {
            var userId = update.CallbackQuery.From.Id;
            var chatId = update.CallbackQuery.Message!.Chat.Id;
            var userSession = _userSessionManager.GetSession(userId);
            var text = update.CallbackQuery.Data;
            if (string.IsNullOrWhiteSpace(text))
            {
                await _bot.SendMessage(chatId, $"empty callback");
                return;
            }

            var commandWithParams = text.Split(' ');
            var command = _commandFactory.CreateCommand(commandWithParams[0]);
            if (command is null)
            {
                await _bot.SendMessage(chatId, $"Unknown command: {text}");
                return;
            }

            await command.Execute(update);
        }
    }
}
