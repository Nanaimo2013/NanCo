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
        
        // Types
        {"int", ConsoleColor.Green},
        {"float", ConsoleColor.Green},
        {"string", ConsoleColor.Green},
        {"bool", ConsoleColor.Green},
        {"array", ConsoleColor.Green}
    };

    public static readonly string[] BuiltInFunctions = {
        "print",
        "clear",
        "beep",
        "delay",
        "readKey",
        "readLine"
    };
} 