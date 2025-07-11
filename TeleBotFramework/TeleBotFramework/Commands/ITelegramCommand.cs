using TeleBotFramework.Models;

namespace TeleBotFramework.Commands;

public interface ITelegramCommand
{
    public static string? Name { get; }
    public static string? Description { get; }
    public static bool IsPublic => true;
    public Task Execute(UpdateInfo update);
}
