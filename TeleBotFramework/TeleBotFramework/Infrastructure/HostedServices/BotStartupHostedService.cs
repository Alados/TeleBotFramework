using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types;
using TeleBotFramework.Commands;

namespace TeleBotFramework.Infrastructure.HostedServices;

public sealed class BotStartupHostedService(
    IServiceProvider services,
    ITelegramBotClient bot) : IHostedService
{
    private readonly IServiceProvider _services = services;
    private readonly ITelegramBotClient _bot   = bot;

    public async Task StartAsync(CancellationToken ct)
    {
        // создаём scope, чтобы получить scoped-сервисы
        using var scope = _services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<ICommandFactory>();

        var cmds = factory.GetCommandList()
            .Select(c => new BotCommand
            {
                Command     = c.Name.TrimStart('/'),
                Description = c.Description
            });

        await _bot.SetMyCommands(cmds, cancellationToken: ct);
    }

    public Task StopAsync(CancellationToken _) => Task.CompletedTask;
}