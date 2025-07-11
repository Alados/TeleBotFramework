using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TeleBotFramework.Models;

namespace TeleBotFramework.Commands;

public class CommandFactory(IServiceProvider serviceProvider) : ICommandFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITelegramCommand? CreateCommand(string commandName)
    {
        return _serviceProvider.GetKeyedService<ITelegramCommand>(commandName);
    }

    public IList<CommandInfo> GetCommandList()
    {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        return _serviceProvider.GetKeyedServices<ITelegramCommand>(KeyedService.AnyKey)
            .Select(s =>
            {
                var type = s.GetType();
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
                var isPublic = (bool)properties.First(p => p.Name == nameof(ITelegramCommand.IsPublic)).GetValue(null)!;
                if (!isPublic)
                    return null; // Skip non-public commands

                var name = properties.First(p => p.Name == nameof(ITelegramCommand.Name)).GetValue(null) as string;
                var description = properties.First(p => p.Name == nameof(ITelegramCommand.Description)).GetValue(null) as string;

                return new CommandInfo
                {
                    Name = name!,
                    Description = description!
                };
            })
            .Where(x => x is not null)
            .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }
}
