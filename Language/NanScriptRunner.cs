using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace NanCo
{
    public class NanScriptRunner
    {
        private readonly string sourceFile;
        private Dictionary<string, object> variables;

        public NanScriptRunner(string file)
        {
            sourceFile = file;
            variables = new Dictionary<string, object>();
        }

        public void Execute()
        {
            string[] lines = File.ReadAllLines(sourceFile);
            
            try
            {
                foreach (string line in lines)
                {
                    ExecuteLine(line.Trim());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Runtime Error: {ex.Message}");
            }
        }

        private void ExecuteLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) return;

            // Basic command parsing
            if (line.StartsWith("print"))
            {
                var content = line.Substring(5).Trim().Trim('"');
                Console.WriteLine(content);
            }
            else if (line.StartsWith("var"))
            {
                HandleVariableDeclaration(line);
            }
            else if (line.StartsWith("terminal."))
            {
                HandleTerminalCommand(line);
            }
            else if (line.StartsWith("audio."))
            {
                HandleAudioCommand(line);
            }
        }

        private void HandleVariableDeclaration(string line)
        {
            // var name = value
            var parts = line.Substring(4).Split('=');
            if (parts.Length == 2)
            {
                string name = parts[0].Trim();
                string value = parts[1].Trim();
                variables[name] = EvaluateValue(value);
            }
        }

        private object EvaluateValue(string value)
        {
            if (int.TryParse(value, out int intResult))
                return intResult;
            if (float.TryParse(value, out float floatResult))
                return floatResult;
            if (value.StartsWith("\"") && value.EndsWith("\""))
                return value.Trim('"');
            if (value == "true" || value == "false")
                return bool.Parse(value);
            
            return value;
        }

        private void HandleTerminalCommand(string line)
        {
            var command = line.Substring(9).Trim(); // Remove "terminal."
            switch (command)
            {
                case "clear":
                    Console.Clear();
                    break;
                case string s when s.StartsWith("color"):
                    var color = s.Substring(6).Trim();
                    SetTerminalColor(color);
                    break;
                default:
                    throw new Exception($"Unknown terminal command: {command}");
            }
        }

        private void HandleAudioCommand(string line)
        {
            var command = line.Substring(6).Trim();
            switch (command)
            {
                case "beep":
                    if (OperatingSystem.IsWindows())
                    {
                        Console.Beep();
                    }
                    break;
                case string s when s.StartsWith("play"):
                    var sound = s.Substring(5).Trim().Trim('"');
                    NanCo.Audio.PlaySound(sound);
                    break;
                default:
                    throw new Exception($"Unknown audio command: {command}");
            }
        }

        private void SetTerminalColor(string color)
        {
            ConsoleColor consoleColor = color.ToLower() switch
            {
                "green" => ConsoleColor.Green,
                "red" => ConsoleColor.Red,
                "blue" => ConsoleColor.Blue,
                "white" => ConsoleColor.White,
                _ => ConsoleColor.Gray
            };
            Console.ForegroundColor = consoleColor;
        }
    }
} 