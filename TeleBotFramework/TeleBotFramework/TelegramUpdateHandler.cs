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
            var userId = update.Message.From.Id;
            var userSession = _userSessionManager.GetSession(userId);
            var commandName = userSession is null ? update.Message.Text : userSession.Command;
            if (string.IsNullOrWhiteSpace(commandName))
            {
                await _bot.SendMessage(update.Message.Chat.Id, $"Enter command, please");
                return;
            }
            var command = _commandFactory.CreateCommand(commandName);
            if (command is null)
            {
                await _bot.SendMessage(update.Message.Chat.Id, $"Unknown command: {commandName}");
                return;
            }

            await command.Execute(update, userSession);
        }

        if (update.CallbackQuery is { } cb)
        {
            var userId = update.CallbackQuery.From.Id;
            var text = update.CallbackQuery.Data;
            if (string.IsNullOrWhiteSpace(text))
            {
                await _bot.SendMessage(update.Message.Chat.Id, $"empty callback");
                return;
            }

            var commandWithParams = text.Split(' ');
            var command = _commandFactory.CreateCommand(commandWithParams[0]);
            if (command is null)
            {
                await _bot.SendMessage(update.Message.Chat.Id, $"Unknown command: {text}");
                return;
            }

            await command.Execute(update, null);
        }
    }
}
