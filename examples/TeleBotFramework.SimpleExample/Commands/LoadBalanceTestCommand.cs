using TeleBotFramework.Attributes;
using TeleBotFramework.Client;
using TeleBotFramework.Commands;
using TeleBotFramework.Models;

namespace TeleBotFramework.SimpleExample.Commands;

[Command("/load_balance_test", "Execute a lot of requests and validate errors", true)]
public class LoadBalanceTestCommand : ITelegramCommand
{
    private readonly ITeleBotClient _bot;

    public LoadBalanceTestCommand(ITeleBotClient bot)
    {
        _bot = bot;
    }

    public async Task Execute(UpdateInfo update)
    {
        var tasks = new List<Task>();
        foreach (var i in Enumerable.Range(0, 100))
        {
            tasks.Add(_bot.SendMessage(update.ChatId, $"Counter {i}"));
        }

        await Task.WhenAll(tasks);
    }
}
