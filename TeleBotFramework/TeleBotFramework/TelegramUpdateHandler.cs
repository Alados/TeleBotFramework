using TeleBotFramework.Commands;
using TeleBotFramework.Models;
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
            var updateInfo = new UpdateInfo
            {
                UserId = update.Message.From!.Id,
                Username = update.Message.From.Username ?? string.Empty,
                FirstName = update.Message.From.FirstName ?? string.Empty,
                LastName = update.Message.From.LastName ?? string.Empty,
                ChatId = update.Message.Chat.Id,
                MessageId = update.Message.MessageId,
                Text = update.Message.Text,
            };
            if (!string.IsNullOrEmpty(updateInfo.Text) && updateInfo.Text == "/cancel")
            {
                await _commandFactory.CreateCommand("/cancel")!.Execute(updateInfo);
                return;
            }

            var userSession = _userSessionManager.GetSession(updateInfo.UserId);
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

            await command.Execute(updateInfo);
        }

        if (update.CallbackQuery is { } cb)
        {
            var updateInfo = new UpdateInfo
            {
                UserId = update.CallbackQuery.From.Id,
                Username = update.CallbackQuery.From.Username ?? string.Empty,
                FirstName = update.CallbackQuery.From.FirstName ?? string.Empty,
                LastName = update.CallbackQuery.From.LastName ?? string.Empty,
                ChatId = update.CallbackQuery.Message!.Chat.Id,
                MessageId = update.CallbackQuery.Message.Id,
                Text = update.CallbackQuery.Data,
                InlineKeyboardMarkup = update.CallbackQuery.Message.ReplyMarkup,
            };
            if (string.IsNullOrWhiteSpace(updateInfo.Text))
            {
                await _bot.SendMessage(updateInfo.ChatId, $"empty callback");
                return;
            }

            var commandWithParams = updateInfo.Text.Split(' ');
            var command = _commandFactory.CreateCommand(commandWithParams[0]);
            if (command is null)
            {
                await _bot.SendMessage(updateInfo.ChatId, $"Unknown command: {updateInfo.Text}");
                return;
            }

            await command.Execute(updateInfo);
        }
    }
}
