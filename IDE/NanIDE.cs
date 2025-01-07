using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using NanCo.Language;
using NanCo.Games;

namespace NanCo.IDE
{
    public class NanIDE
    {
        private string? currentFile;
        private List<string> codeLines;
        private int cursorX;
        private int cursorY;
        private int scrollOffset;
        private bool isModified;
        private int maxDisplayLines;

        public NanIDE(string? filePath = null)
        {
            currentFile = filePath;
            codeLines = new List<string> { "" };
            cursorX = 0;
            cursorY = 0;
            scrollOffset = 0;
            maxDisplayLines = Console.WindowHeight - 7;
            
            if (filePath != null && File.Exists(filePath))
            {
                LoadFile(filePath);
            }
        }

        public void Start()
        {
            Console.Clear();
            while (true)
            {
                DrawInterface();
                var key = Console.ReadKey(true);

                if (key.Modifiers == ConsoleModifiers.Control)
                {
                    if (HandleControlKeys(key.Key)) return;
                }
                else
                {
                    HandleRegularKeys(key);
                }
            }
        }

        private void HandleRegularKeys(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (cursorX > 0) cursorX--;
                    break;

                case ConsoleKey.RightArrow:
                    if (cursorX < codeLines[cursorY].Length) cursorX++;
                    break;

                case ConsoleKey.UpArrow:
                    if (cursorY > 0)
                    {
                        cursorY--;
                        if (cursorY < scrollOffset) scrollOffset--;
                        cursorX = Math.Min(cursorX, codeLines[cursorY].Length);
                    }
                    break;

                case ConsoleKey.DownArrow:
                    if (cursorY < codeLines.Count - 1)
                    {
                        cursorY++;
                        if (cursorY >= scrollOffset + maxDisplayLines) scrollOffset++;
                        cursorX = Math.Min(cursorX, codeLines[cursorY].Length);
                    }
                    break;

                case ConsoleKey.Enter:
                    string currentLine = codeLines[cursorY];
                    string restOfLine = currentLine.Substring(cursorX);
                    codeLines[cursorY] = currentLine.Substring(0, cursorX);
                    codeLines.Insert(cursorY + 1, GetIndentation(currentLine) + restOfLine);
                    cursorY++;
                    cursorX = GetIndentation(currentLine).Length;
                    isModified = true;
                    break;

                case ConsoleKey.Backspace:
                    if (cursorX > 0)
                    {
                        codeLines[cursorY] = codeLines[cursorY].Remove(cursorX - 1, 1);
                        cursorX--;
                        isModified = true;
                    }
                    else if (cursorY > 0)
                    {
                        cursorX = codeLines[cursorY - 1].Length;
                        codeLines[cursorY - 1] += codeLines[cursorY];
                        codeLines.RemoveAt(cursorY);
                        cursorY--;
                        isModified = true;
                    }
                    break;

                default:
                    if (key.KeyChar >= 32 && key.KeyChar <= 126)
                    {
                        codeLines[cursorY] = codeLines[cursorY].Insert(cursorX, key.KeyChar.ToString());
                        cursorX++;
                        isModified = true;
                    }
                    break;
            }
        }

        private bool HandleControlKeys(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.S:
                    SaveFile();
                    return false;

                case ConsoleKey.R:
                    RunCode();
                    return false;

                case ConsoleKey.Q:
                    return PromptExit();

                case ConsoleKey.P:
                    var projectView = new ProjectView(Path.GetDirectoryName(currentFile));
                    projectView.DisplayProjectTree();
                    while (true)
                    {
                        var projectKey = Console.ReadKey(true);
                        if (projectKey.Key == ConsoleKey.Escape) break;
                        projectView.HandleInput(projectKey);
                    }
                    return false;

                case ConsoleKey.G:
                    InsertGameTemplate();
                    return false;

                default:
                    return false;
            }
        }

        private void RunCode()
        {
            if (currentFile == null) return;
            
            SaveFile();
            Console.Clear();
            Console.WriteLine("=== Running Program ===\n");
            
            try
            {
                var runner = new NanScriptRunner(currentFile);
                runner.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to return to IDE...");
            Console.ReadKey(true);
        }

        private void LoadFile(string path)
        {
            currentFile = path;
            codeLines = File.ReadAllLines(path).ToList();
            cursorX = 0;
            cursorY = 0;
            scrollOffset = 0;
            isModified = false;
        }

        private void DrawInterface()
        {
            // Save cursor position
            var originalX = Console.CursorLeft;
            var originalY = Console.CursorTop;
            
            // Clear only the necessary parts without full screen clear
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"NanCo IDE - {currentFile ?? "New File"}".PadRight(Console.WindowWidth));
            Console.WriteLine("Ctrl+S: Save | Ctrl+R: Run | Ctrl+P: Project | Ctrl+Q: Quit".PadRight(Console.WindowWidth));
            Console.WriteLine("─".PadRight(Console.WindowWidth, '─'));

            for (int i = scrollOffset; i < Math.Min(codeLines.Count, scrollOffset + maxDisplayLines); i++)
            {
                Console.SetCursorPosition(0, i - scrollOffset + 3);
                Console.Write($"{(i + 1).ToString().PadLeft(4)} │ ");
                HighlightSyntax(codeLines[i]);
                Console.Write(new string(' ', Console.WindowWidth - Console.CursorLeft));
            }

            // Restore cursor position
            Console.SetCursorPosition(cursorX + 6, cursorY - scrollOffset + 3);
        }

        private void HighlightSyntax(string line)
        {
            var words = line.Split(' ');
            foreach (var word in words)
            {
                if (NanSyntax.Keywords.ContainsKey(word))
                {
                    Console.ForegroundColor = NanSyntax.Keywords[word];
                    Console.Write(word + " ");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(word + " ");
                }
            }
        }

        private string GetIndentation(string line)
        {
            int spaces = 0;
            foreach (char c in line)
            {
                if (c == ' ') spaces++;
                else break;
            }
            return new string(' ', spaces);
        }

        private void SaveFile()
        {
            if (string.IsNullOrEmpty(currentFile))
            {
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                Console.Write("Enter filename: ");
                currentFile = Console.ReadLine();
            }

            try
            {
                File.WriteAllLines(currentFile, codeLines);
                isModified = false;
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                Console.Write("File saved successfully!");
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                Console.Write($"Error saving file: {ex.Message}");
                Thread.Sleep(2000);
            }
        }

        private bool PromptExit()
        {
            if (!isModified) return true;
            
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write("Save changes before exit? (Y/N): ");
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Y)
            {
                SaveFile();
            }
            return true;
        }

        private void HandleInput(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Tab)
            {
                string currentLine = codeLines[cursorY];
                string word = GetWordBeforeCursor(currentLine);
                
                if (NanSyntax.CodeSnippets.ContainsKey(word))
                {
                    InsertSnippet(word);
                    DrawInterface();
                }
            }
            // ... rest of input handling
        }

        private void InsertSnippet(string keyword)
        {
            string snippet = NanSyntax.CodeSnippets[keyword];
            string[] snippetLines = snippet.Split('\n');
            
            codeLines.RemoveAt(cursorY);
            foreach (var line in snippetLines.Reverse())
            {
                codeLines.Insert(cursorY, line);
            }
        }

        private string GetWordBeforeCursor(string line)
        {
            int start = cursorX;
            while (start > 0 && char.IsLetterOrDigit(line[start - 1]))
            {
                start--;
            }
            return line.Substring(start, cursorX - start);
        }

        private void InsertGameTemplate()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write("Select template (1: Basic, 2: Arcade): ");
            var choice = Console.ReadKey(true);
            
            string template = choice.KeyChar == '1' ? 
                GameManager.GameTemplates["basic"] : 
                GameManager.GameTemplates["arcade"];
            
            codeLines.Clear();
            codeLines.AddRange(template.Split('\n'));
            cursorX = 0;
            cursorY = 0;
            isModified = true;
        }
    }
} 