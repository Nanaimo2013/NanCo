using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace NanCo
{
    public class SimpleTextEditor
    {
        private string filePath;
        private List<string> lines;
        private int currentLine;
        private int cursorX;

        public SimpleTextEditor(string path)
        {
            filePath = path;
            lines = new List<string>();
            LoadFile();
        }

        private void LoadFile()
        {
            if (File.Exists(filePath))
            {
                lines = File.ReadAllLines(filePath).ToList();
            }
        }

        public void Start()
        {
            Console.Clear();
            Console.WriteLine($"NanCo Text Editor - {filePath}");
            Console.WriteLine("Ctrl+S: Save | Ctrl+Q: Quit");
            Console.WriteLine("─".PadRight(Console.WindowWidth, '─'));

            DisplayContent();

            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Modifiers == ConsoleModifiers.Control)
                {
                    if (key.Key == ConsoleKey.S)
                    {
                        SaveFile();
                        continue;
                    }
                    else if (key.Key == ConsoleKey.Q)
                    {
                        if (PromptSave()) break;
                        continue;
                    }
                }

                HandleKeyPress(key);
                DisplayContent();
            }
        }

        private void DisplayContent()
        {
            Console.SetCursorPosition(0, 3);
            for (int i = 0; i < lines.Count; i++)
            {
                Console.WriteLine($"{(i + 1).ToString().PadLeft(4)} │ {lines[i]}".PadRight(Console.WindowWidth));
            }
        }

        private void SaveFile()
        {
            try
            {
                File.WriteAllLines(filePath, lines);
                ShowMessage("File saved successfully!");
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving file: {ex.Message}");
            }
        }

        private void ShowMessage(string message)
        {
            int currentTop = Console.CursorTop;
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write(message.PadRight(Console.WindowWidth));
            Console.SetCursorPosition(cursorX, currentTop);
        }

        private bool PromptSave()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write("Save changes? (Y/N): ");
            var response = Console.ReadKey(true);
            if (response.Key == ConsoleKey.Y)
            {
                SaveFile();
            }
            return true;
        }

        private void HandleKeyPress(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    lines.Insert(currentLine + 1, "");
                    currentLine++;
                    cursorX = 0;
                    break;

                case ConsoleKey.Backspace:
                    if (cursorX > 0)
                    {
                        lines[currentLine] = lines[currentLine].Remove(cursorX - 1, 1);
                        cursorX--;
                    }
                    break;

                default:
                    if (key.KeyChar >= 32 && key.KeyChar <= 126)
                    {
                        if (currentLine >= lines.Count)
                            lines.Add("");
                        lines[currentLine] = lines[currentLine].Insert(cursorX, key.KeyChar.ToString());
                        cursorX++;
                    }
                    break;
            }
        }
    }
} 