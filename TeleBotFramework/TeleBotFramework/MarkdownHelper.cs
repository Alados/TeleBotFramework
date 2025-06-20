namespace TeleBotFramework;
public static class MarkdownHelper
{
    public static string EscapeForMarkdownV2(this string text)
    {
        var charsToEscape = new[] { "_", "[", "]", "(", ")", "~", "`", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!" };
        foreach (var c in charsToEscape)
            text = text.Replace(c, $"\\{c}");
        return text;
    }
}
