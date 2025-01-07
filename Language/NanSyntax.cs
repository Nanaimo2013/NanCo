using System;
using System.Collections.Generic;

public static class NanSyntax
{
    public static readonly Dictionary<string, ConsoleColor> Keywords = new()
    {
        // Standard keywords
        {"func", ConsoleColor.Blue},
        {"var", ConsoleColor.Blue},
        {"const", ConsoleColor.Blue},
        {"if", ConsoleColor.Magenta},
        {"else", ConsoleColor.Magenta},
        {"loop", ConsoleColor.Magenta},
        {"break", ConsoleColor.Magenta},
        {"return", ConsoleColor.Magenta},
        
        // NanCo specific keywords
        {"terminal", ConsoleColor.Cyan},
        {"screen", ConsoleColor.Cyan},
        {"audio", ConsoleColor.Cyan},
        {"input", ConsoleColor.Cyan},
        {"game", ConsoleColor.Cyan},
        
        // Types
        {"int", ConsoleColor.Green},
        {"float", ConsoleColor.Green},
        {"string", ConsoleColor.Green},
        {"bool", ConsoleColor.Green},
        {"array", ConsoleColor.Green},
        
        // Additional programming keywords
        {"class", ConsoleColor.Blue},
        {"extends", ConsoleColor.Blue},
        {"public", ConsoleColor.Blue},
        {"private", ConsoleColor.Blue},
        {"static", ConsoleColor.Blue},
        {"new", ConsoleColor.Blue},
        {"override", ConsoleColor.Blue},
        {"virtual", ConsoleColor.Blue},
        {"protected", ConsoleColor.Blue}
    };

    public static readonly string[] BuiltInFunctions = {
        "print",
        "clear",
        "beep",
        "delay",
        "readKey",
        "readLine",
        "DrawBorder",
        "Update",
        "Render"
    };

    public static readonly Dictionary<string, string> CodeSnippets = new()
    {
        {"class", "class ClassName\n{\n    // Properties\n    \n    // Methods\n    \n}"},
        {"func", "func FunctionName()\n{\n    \n}"},
        {"game", "class NewGame : BaseGame\n{\n    public override string Name => \"GameName\";\n    public override string Description => \"Game Description\";\n}"}
    };
}