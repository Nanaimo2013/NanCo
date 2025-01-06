using System;
using System.IO;

namespace NanCo
{
    static class CommandProcessor
    {
        public static void ProcessCommand(string command, Terminal terminal)
        {
            var args = command.Split(' ');
            var cmd = args[0].ToLower();

            switch (cmd)
            {
                case "help":
                    DisplayHelp();
                    break;
                    
                case "new":
                    if (args.Length > 2 && args[1] == "project")
                    {
                        ProjectManager.CreateProject(args[2]);
                    }
                    else
                    {
                        Console.WriteLine("Usage: new project <name>");
                    }
                    break;

                case "run":
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
            
            Console.WriteLine("\nGames & Apps:");
            Console.WriteLine("  dino            - Play Dino game");
            Console.WriteLine("  games           - List available games");
            Console.WriteLine("  run game <name> - Run specific game");
            
            Console.WriteLine("\nSystem Commands:");
            Console.WriteLine("  clear           - Clear screen");
            Console.WriteLine("  date            - Show current date");
            Console.WriteLine("  time            - Show current time");
            Console.WriteLine("  exit            - Exit terminal");
        }
    }
}

