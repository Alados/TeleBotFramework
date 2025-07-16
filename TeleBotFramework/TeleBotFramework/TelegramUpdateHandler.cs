using Microsoft.Extensions.Logging;
using TeleBotFramework.Commands;
using TeleBotFramework.Models;
using TeleBotFramework.StateManager;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleBotFramework;
public class TelegramUpdateHandler : ITelegramUpdateHandler
{
    private readonly ICommandFactory _commandFactory;
    private readonly IUserSessionManager _userSessionManager;
    private readonly ILogger<TelegramUpdateHandler> _logger;

    public TelegramUpdateHandler(ICommandFactory commandFactory, IUserSessionManager userSessionManager, ILogger<TelegramUpdateHandler> logger)
    {
        _commandFactory = commandFactory;
        _userSessionManager = userSessionManager;
        _logger = logger;
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
                _logger.LogInformation("Unknown command {commandName}", commandName);
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
                _logger.LogInformation("empty callback");
                return;
            }

            var commandWithParams = updateInfo.Text.Split(' ');
            var command = _commandFactory.CreateCommand(commandWithParams[0]);
            if (command is null)
            {

                _logger.LogInformation("Unknown command {commandName}", updateInfo.Text);
                return;
            }

            await command.Execute(updateInfo);
        }
    }
}
