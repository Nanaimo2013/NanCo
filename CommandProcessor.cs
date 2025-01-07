using System;
using System.IO;
using System.Collections.Generic;
using NanCo.Language;
using NanCo.FileSystem;
using NanCo.IDE;
using NanCo.Models;
using NanCo.Games;

namespace NanCo
{
    static class CommandProcessor
    {
        private static Dictionary<string, Action> customCommands = new();

        public static void RegisterCustomCommand(string name, Action action)
        {
            if (!customCommands.ContainsKey(name))
            {
                customCommands.Add(name, action);
            }
        }

        public static void ProcessCommand(string command, Terminal terminal)
        {
            var args = command.Split(' ');
            var cmd = args[0].ToLower();

            switch (cmd)
            {
                case "help":
                    DisplayHelp();
                    break;
                    
                case "ide":
                    var ide = new NanIDE();
                    ide.Start();
                    break;

                case "new":
                    if (args.Length > 1)
                    {
                        if (args[1].ToLower() == "project")
                        {
                            if (args.Length > 2)
                            {
                                ProjectManager.CreateProject(args[2]);
                            }
                            else
                            {
                                Console.WriteLine("Usage: new project <name>");
                            }
                        }
                        else if (args[1].ToLower() == "file")
                        {
                            var editor = new NanIDE();
                            editor.Start();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Usage: new [file|project] <name>");
                    }
                    break;

                case "run":
                    if (args.Length > 2 && args[1].ToLower() == "game")
                    {
                        var games = new GameManager();
                        var gameName = args[2].ToLower();
                        var gameIndex = games.Games.FindIndex(g => g.Name.ToLower() == gameName);
                        if (gameIndex >= 0)
                        {
                            games.StartGame(gameIndex);
                        }
                        else
                        {
                            Console.WriteLine($"Game '{args[2]}' not found. Use 'games' to see available games.");
                        }
                    }
                    else
                    {
                        if (args.Length > 1)
                        {
                            string filePath = args[1];
                            if (Path.GetExtension(filePath).ToLower() == ".ns")
                            {
                                var runner = new NanScriptRunner(filePath);
                                runner.Execute();
                            }
                            else
                            {
                                Console.WriteLine("Can only run .ns files");
                            }
                        }
                    }
                    break;

                case "edit":
                    Console.Write("Enter file path: ");
                    string editPath = Console.ReadLine();
                    OpenFileWithAppropriateEditor(editPath);
                    break;

                case "files":
                    FileSystemManager.ListFiles();
                    break;

                case "cd":
                    if (args.Length > 1)
                        Directory.SetCurrentDirectory(args[1]);
                    else
                        Console.WriteLine("Please specify a directory");
                    break;

                case "clear":
                    Console.Clear();
                    break;

                case "exit":
                    Environment.Exit(0);
                    break;

                case "games":
                    var gameManager = new GameManager();
                    gameManager.ListGames();
                    Console.Write("\nSelect a game (number) or press Enter to cancel: ");
                    if (int.TryParse(Console.ReadLine(), out int selection))
                    {
                        gameManager.StartGame(selection - 1);
                    }
                    break;

                case "dino":
                    var dinoGame = new DinoGame();
                    dinoGame.Start();
                    break;

                case "snake":
                    var snakeGame = new SnakeGame();
                    snakeGame.Start();
                    break;

                default:
                    Console.WriteLine("Unknown command. Type 'help' for assistance.");
                    break;
            }
        }

        private static void OpenFileWithAppropriateEditor(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("No file specified.");
                return;
            }

            string extension = Path.GetExtension(filePath).ToLower();
            string fullPath = Path.GetFullPath(filePath);

            switch (extension)
            {
                case ".ns":  // NanCo Script
                case ".nsp": // NanCo Project
                    var ide = new NanIDE(fullPath);
                    ide.Start();
                    break;
                    
                default:
                    var editor = new SimpleTextEditor(fullPath);
                    editor.Start();
                    break;
            }
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("\n=== NanCo Terminal Help ===");
            Console.WriteLine("\nFile Operations:");
            Console.WriteLine("  edit <file>     - Open file in text editor or IDE");
            Console.WriteLine("  files           - List files in current directory");
            Console.WriteLine("  new project     - Create new NanCo project (.nsp)");
            Console.WriteLine("  new file        - Create new NanCo script (.ns)");
            
            Console.WriteLine("\nDevelopment Tools:");
            Console.WriteLine("  ide             - Open NanCo IDE");
            Console.WriteLine("  compile <file>  - Compile NanCo script");
            Console.WriteLine("  run <file>      - Run NanCo script or project");
            
            Console.WriteLine("\nGames & Entertainment:");
            Console.WriteLine("  games           - List and play available games");
            Console.WriteLine("  snake           - Play Snake game");
            Console.WriteLine("  dino            - Play Dino game");
            Console.WriteLine("  run game <name> - Run specific game by name");
            
            Console.WriteLine("\nSystem Commands:");
            Console.WriteLine("  clear           - Clear screen");
            Console.WriteLine("  date            - Show current date");
            Console.WriteLine("  time            - Show current time");
            Console.WriteLine("  exit            - Exit terminal");
        }
    }
}

