using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TeleBotFramework;
using TeleBotFramework.SimpleExample.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
builder.Configuration.AddJsonFile("appsettings.json", optional: false);
builder.Configuration.AddJsonFile($"appsettings.{env}.json", optional: true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<ITelegramBotClient>(provider =>
{
    var token = provider.GetRequiredService<IConfiguration>()["Telegram:Token"];
    if (string.IsNullOrEmpty(token))
    {
        throw new ArgumentException("Telegram bot token is not configured in appsettings.json or environment variables.");
    }
    return new TelegramBotClient(token);
});
builder.Services.AddTelegramFramework([typeof(StartCommand).Assembly]);

var app = builder.Build();
app.UseExceptionHandler(options =>
{
    options.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        var exception = context.Features.Get<IExceptionHandlerFeature>();
        if (exception != null)
        {
            var message = $"{exception.Error.Message}";
            await context.Response.WriteAsync(message).ConfigureAwait(false);
        }
    });
});

app.MapPost("/bot/webhook", async (
    [FromBody] Update update,
    [FromServices] ITelegramUpdateHandler updateHandler) =>
{
    await updateHandler.HandleUpdate(update);
    return Results.Ok();
});

app.Run();