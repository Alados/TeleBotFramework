using TeleBotFramework.Models;

namespace TeleBotFramework.Commands;

public interface ITelegramCommand
{
    public Task Execute(UpdateInfo update);
}
