using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TeleBotFramework.Attributes;
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
                var command = type.GetCustomAttribute<CommandAttribute>() ?? throw new Exception("ITelegramCommand should have Command attribute");
                if (!command.IsPublic)
                    return null; // Skip non-public commands

                return new CommandInfo
                {
                    Name = command.Name,
                    Description = command.Description
                };
            })
            .Where(x => x is not null)
            .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
    }
}
