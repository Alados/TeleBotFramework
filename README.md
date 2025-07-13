# TeleBotFramework

Minimalistic, dependency-injection-first command framework for [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot), designed to let you build modular, maintainable Telegram bots with almost zero boilerplate.

---

## ✨ Features

- **ITelegramCommand**-based commands – just implement the interface  
- **Auto-registration** of all commands found in your assemblies  
- **IServiceCollection** extension: `AddTelegramFramework(...)`  
- **ITelegramUpdateHandler** dispatches incoming updates to your commands
- **IUserStateManager** for per-user state persistence across sessions  
- **MarkdownHelper** utility to safely format Markdown messages  
- Ready to use in **ASP.NET Core** (webhook) or **long-polling** console apps

---

## ⚙️ Installation

dotnet add package TeleBotFramework

## 🚀 Usage

### 1. Configure your bot in ASP.NET Core (webhook)
```
csharp
Copy
Edit
using Microsoft.AspNetCore.Mvc;
using TeleBotFramework;
using Telegram.Bot;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Register your TelegramBotClient
builder.Services.AddSingleton<ITelegramBotClient>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var token = cfg["Telegram:Token"];
    if (string.IsNullOrWhiteSpace(token))
        throw new ArgumentException("Telegram:Token is missing");
    return new TelegramBotClient(token);
});

// 2️⃣ Add MVC & the framework, scanning your command assembly
builder.Services.AddControllers();
builder.Services.AddTelegramFramework(typeof(StartCommand).Assembly);

var app = builder.Build();

// 3️⃣ Map the webhook endpoint
app.MapPost("/bot/webhook", async (
    [FromBody] Update update,
    [FromServices] ITelegramUpdateHandler handler) =>
{
    await handler.HandleUpdate(update);
    return Results.Ok();
});

app.Run();
```

### 2. Define a command
Create a class in your project (e.g. under Commands/) that implements ITelegramCommand:

```
using TeleBotFramework.Commands;
using TeleBotFramework.Models;
using Telegram.Bot;

namespace TeleBotFramework.SimpleExample.Commands
internal class StartCommand(ITelegramBotClient bot) : ITelegramCommand
{
    private readonly ITelegramBotClient _bot = bot;

    public static string Name => "/start";
    public static string Description => "Start operation";
    public static bool IsPublic => true;

    public async Task Execute(UpdateInfo update)
    {
        await _bot.SendMessage(update.ChatId, $"Hello, {update.Username ?? update.FirstName + " " + update.LastName}");
    }
}

```

* Name - command name to call it
* Description - will be shown in bot menu

### 3. Run the example
A complete ASP.NET Core example lives under:
    examples/TeleBotFramework.SimpleExample/
    
### To try it out:

```
git clone https://github.com/Alados/TeleBotFramework.git
cd TeleBotFramework/examples/TeleBotFramework.SimpleExample
dotnet run
```

### Available on Nuget
Link - https://www.nuget.org/packages/TeleBotFramework/

## 📂 Repository structure
```
/TeleBotFramework/                  # Core library
  /Commands/                        # ITelegramCommand + any built-in commands
  /Handlers/                        # Update dispatching & routing
  /Models/                          # UpdateInfo, other DTOs
  /Extensions/                      # IServiceCollection extensions
  /State/                           # IUserStateManager implementations
  /Utils/                           # MarkdownHelper and other helpers
/examples/
  /TeleBotFramework.SimpleExample/  # ASP.NET Core webhook sample
.gitignore
README.md
LICENSE
```

## 🤝 Contributing
Contributions, issues and feature requests are welcome!
* Fork the repo
* Create your feature branch (git checkout -b feature/foo)
* Commit your changes (git commit -am "Add foo")
* Push to the branch (git push origin feature/foo)
* Open a Pull Request

## 📄 License
This project is licensed under the MIT License. See LICENSE for details.
