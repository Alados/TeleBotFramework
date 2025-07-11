using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TeleBotFramework.Commands;
using TeleBotFramework.StateManager;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeleBotFramework;
public static class ModuleExtensions
{
    public static void AddTelegramFramework(this IServiceCollection services, Assembly[] assembliesToRegisterCommandFrom)
    {
        services.AddScoped<ICommandFactory, CommandFactory>();
        services.AddScoped<ITelegramUpdateHandler, TelegramUpdateHandler>();
        services.AddSingleton<IUserSessionManager, UserSessionManager>();
        services.AutoRegisterCommands([.. assembliesToRegisterCommandFrom, typeof(ModuleExtensions).Assembly]);
    }

    private static void AutoRegisterCommands(this IServiceCollection services, Assembly[] assemblies)
    {
        var type = typeof(ITelegramCommand);
        var commandTypes = assemblies.SelectMany(s => s.GetTypes()).Where((p) => type.IsAssignableFrom(p) && p != type).ToList();
        var commandList = new List<(string Name, string Description)>();
        foreach (var commandType in commandTypes)
        {
            if (!typeof(ITelegramCommand).IsAssignableFrom(commandType))
                continue;

            var nameProperty = commandType.GetProperty(nameof(ITelegramCommand.Name), BindingFlags.Public | BindingFlags.Static);
            var name = (string)nameProperty!.GetValue(null)!;

            var descriptionProperty = commandType.GetProperty(nameof(ITelegramCommand.Description), BindingFlags.Public | BindingFlags.Static);
            var description = (string)descriptionProperty!.GetValue(null)!;

            var isPublicProperty = commandType.GetProperty(nameof(ITelegramCommand.IsPublic), BindingFlags.Public | BindingFlags.Static);
            var isPublic = (bool)isPublicProperty!.GetValue(null)!;
            if (isPublic)
                commandList.Add((name, description));

            services.AddKeyedScoped(typeof(ITelegramCommand), name, commandType);
        }

        var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var bot = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        bot.SetMyCommands(
            [.. commandList.Select(c =>
                new BotCommand
                {
                    Command = c.Name.TrimStart('/'), // /start → start
                    Description = c.Description
                })
            ]
        );
    }
}
