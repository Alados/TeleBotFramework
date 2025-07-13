namespace TeleBotFramework.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class CommandAttribute(string name, string description, bool isPublic = false) : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public bool IsPublic { get; } = isPublic;
}
