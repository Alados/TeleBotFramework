using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TeleBotFramework.Attributes;
using TeleBotFramework.Client;
using TeleBotFramework.Commands;
using TeleBotFramework.Infrastructure.HostedServices;
using TeleBotFramework.StateManager;

namespace TeleBotFramework;
public static class ModuleExtensions
{
    public static void AddTelegramFramework(this IServiceCollection services, Assembly[] assembliesToRegisterCommandFrom)
    {
        services.AddScoped<ICommandFactory, CommandFactory>();
        services.AddScoped<ITelegramUpdateHandler, TelegramUpdateHandler>();
        services.AddSingleton<IUserSessionManager, UserSessionManager>();
        services.AddScoped<ITeleBotClient, TeleBotClient>();
        services.AutoRegisterCommands([.. assembliesToRegisterCommandFrom, typeof(ModuleExtensions).Assembly]);
        services.AddHostedService<BotStartupHostedService>();
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

            var command = commandType.GetCustomAttribute<CommandAttribute>() ?? throw new Exception($"ITelegramCommand should have Command attribute");
            if (command.IsPublic)
                commandList.Add((command.Name, command.Description));

            services.AddKeyedScoped(typeof(ITelegramCommand), command.Name, commandType);
        }
    }
}
