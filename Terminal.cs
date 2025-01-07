using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using NanCo.FileSystem;
using NanCo.Language;
using NanCo.IDE;
using NanCo.Games;
using NanCo;

namespace NanCo
{
    class Terminal
    {
        private string currentUser = "";
        public string prompt = "NanCo$ ";
        private readonly string usersFile = Path.Combine(FileSystemManager.ConfigDir, "users.dat");
        private readonly string versionFile = Path.Combine(FileSystemManager.ConfigDir, "version.txt");
        private Dictionary<string, string> users = new Dictionary<string, string>();

        public void Run()
        {
            try
            {
                InitializeConsole();
                InitializeGames();
                
                LoadUsers();
                
                if (!AuthenticateUser())
                {
                    return;
                }

                DisplayTerminalLogo();
                DisplayWelcomeMessage();

                RunCommandLoop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Terminal error: {ex.Message}");
            }
        }

        private void InitializeConsole()
        {
            Thread.Sleep(100);

            try
            {
                if (OperatingSystem.IsWindows())
                {
                    NativeMethods.AllocConsole();
                }

                Console.Title = "NanCo Terminal";
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.BackgroundColor = ConsoleColor.Black;
                SafeClear();
            }
            catch
            {
                // Ignore console property errors
            }
        }

        private void SafeClear()
        {
            try
            {
                Console.Clear();
            }
            catch
            {
                // If Clear fails, try to write newlines
                try
                {
                    string blank = new string(' ', Console.WindowWidth);
                    for (int i = 0; i < Console.WindowHeight; i++)
                    {
                        Console.WriteLine(blank);
                    }
                    Console.SetCursorPosition(0, 0);
                }
                catch
                {
                    // If all else fails, just write some newlines
                    for (int i = 0; i < 50; i++)
                    {
                        Console.WriteLine();
                    }
                }
            }
        }

        private void RunCommandLoop()
        {
            string input;
            do
            {
                try
                {
                    Console.Write($"{currentUser}@{prompt}");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        CommandProcessor.ProcessCommand(input, this);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Command error: {ex.Message}");
                    input = null;
                }
            } while (input?.ToLower() != "exit");
        }

        private string GetVersion()
        {
            return VersionManager.GetVersion();
        }

        private void DisplayTerminalLogo()
        {
            Console.Clear();
            Console.WriteLine();
            Effects.TypewriterEffect("Initializing Terminal Interface...");
            Effects.LoadingBar(20);
            Console.WriteLine(" [OK]");

            Console.WriteLine();
            Effects.TypewriterEffect("Loading User Profile...");
            Effects.LoadingBar(15);
            Console.WriteLine(" [OK]");

            Console.WriteLine();
            Effects.TypewriterEffect("Configuring Environment...");
            Effects.LoadingBar(25);
            Console.WriteLine(" [OK]");

            Thread.Sleep(500);
            Console.WriteLine();
            Console.Clear();
            string terminalLogo = @"
    ███╗   ██╗ █████╗ ███╗   ██╗███████╗    
    ████╗  ██║██╔══██╗████╗  ██║██╔════╝    
    ██╔██╗ ██║███████║██╔██╗ ██║███████╗    
    ██║╚██╗██║██╔══██║██║╚██╗██║╚════██║    
    ██║ ╚████║██║  ██║██║ ╚████║███████║    
    ╚═╝  ╚═══╝╚═╝  ╚═╝╚═╝  ╚═══╝╚══════╝    
    ███████╗████████╗██╗   ██╗██████╗ ██╗ ██████╗ 
    ██╔════╝╚══██╔══╝██║   ██║██╔══██╗██║██╔═══██╗
    ███████╗   ██║   ██║   ██║██║  ██║██║██║   ██║
    ╚════██║   ██║   ██║   ██║██║  ██║██║██║   ██║
    ███████║   ██║   ╚██████╔╝██████╔╝██║╚██████╔╝
    ╚══════╝   ╚═╝    ╚═════╝ ╚═════╝ ╚═╝ ╚═════╝ ";

            foreach (var line in terminalLogo.Split('\n'))
            {
                Console.WriteLine(line);
                Audio.PlayBeep(new Random().Next(300, 800), 10);
                Thread.Sleep(50);
            }
        }

    private void DisplayWelcomeMessage()
    {
        string version = GetVersion();
        string welcomeBox = $@"
     ╔════════════════════════════════════════════════╗
     ║              NanS Studio Terminal              ║
     ║             Version {version,-28}║
     ║                                                ║
     ║              Welcome, {currentUser,-32}║
     ║           Type 'help' for assistance           ║
     ╚════════════════════════════════════════════════╝ ";

        Console.WriteLine(welcomeBox);
        Console.WriteLine("\n");
    }

        private bool AuthenticateUser()
        {
            while (true)
            {
                Console.Clear();
                DisplayAuthenticationLogo();
                Console.WriteLine("\n1. Login");
                Console.WriteLine("2. Create New User");
                Console.WriteLine("3. Exit");
                Console.Write("\nSelect an option (1-3): ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        if (Login())
                            return true;
                        break;
                    case "2":
                        CreateNewUser();
                        break;
                    case "3":
                        return false;
                    default:
                        Console.WriteLine("Invalid option. Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                }
            }
        }

        private bool Login()
        {
            Console.Clear();
            DisplayAuthenticationLogo();
            Console.Write("\nUsername: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = ReadPassword();

            if (users.TryGetValue(username, out string storedPassword) && storedPassword == password)
            {
                currentUser = username;
                prompt = $"NanCo/{username}$ ";
                Effects.TypewriterEffect($"\nWelcome back, {username}!");
                Effects.LoadingBar(20);
                Console.WriteLine(" [AUTHENTICATED]");
                Thread.Sleep(1000);
                return true;
            }

            Console.WriteLine("\nInvalid username or password. Press any key to continue...");
            Console.ReadKey(true);
            return false;
        }

        private void CreateNewUser()
        {
            Console.Clear();
            DisplayAuthenticationLogo();
            Console.Write("\nNew Username: ");
            string username = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username) || users.ContainsKey(username))
            {
                Console.WriteLine("\nInvalid or existing username. Press any key to continue...");
                Console.ReadKey(true);
                return;
            }

            Console.Write("Password: ");
            string password = ReadPassword();

            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("\nInvalid password. Press any key to continue...");
                Console.ReadKey(true);
                return;
            }

            users[username] = password;
            SaveUsers();

            Effects.TypewriterEffect("\nUser created successfully!");
            Effects.LoadingBar(20);
            Console.WriteLine(" [COMPLETE]");
            Thread.Sleep(1000);
        }

        private string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[..^1];
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            return password;
        }

        private void LoadUsers()
        {
            try
            {
                // Create Config directory if it doesn't exist
                string configDir = Path.GetDirectoryName(usersFile);
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }

                // Create users file if it doesn't exist
                if (!File.Exists(usersFile))
                {
                    Console.WriteLine("\nInitializing user database...");
                    Effects.LoadingBar(20);
                    
                    // Create default users
                    List<string> defaultUsers = new List<string>
                    {
                        "admin:password",
                        "guest:guest"
                    };
                    
                    File.WriteAllLines(usersFile, defaultUsers);
                    
                    Console.WriteLine(" [CREATED]");
                    Audio.PlayWarning();
                    
                    Effects.TypewriterEffect("\nDefault credentials created:");
                    Console.WriteLine("\n  Username: admin");
                    Console.WriteLine("  Password: password");
                    Console.WriteLine("\nPlease change these credentials after logging in!");
                    
                    Audio.PlayWarning();
                    Thread.Sleep(2000);
                }

                // Load users from file
                string[] lines = File.ReadAllLines(usersFile);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        users[parts[0]] = parts[1];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError initializing users: {ex.Message}");
                Audio.PlayError();
                Thread.Sleep(2000);
            }
        }

        private void SaveUsers()
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (var user in users)
                {
                    lines.Add($"{user.Key}:{user.Value}");
                }
                File.WriteAllLines(usersFile, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving users: {ex.Message}");
            }
        }

        private void DisplayAuthenticationLogo()
        {
            string logo = @"
    ███╗   ██╗ █████╗ ███╗   ██╗ ██████╗ ██████╗ 
    ████╗  ██║██╔══██╗████╗  ██║██╔════╝██╔═══██╗
    ██╔██╗ ██║███████║██╔██╗ ██║██║     ██║   ██║
    ██║╚██╗██║██╔══██║██║╚██╗██║██║     ██║   ██║
    ██║ ╚████║██║  ██║██║ ╚████║╚██████╗╚██████╔╝
    ╚═╝  ╚═══╝╚═╝  ╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚═════╝ 
    
     ╔═══════════════════════════════════════════╗
     ║        Terminal Authentication            ║
     ╚═══════════════════════════════════════════╝";

            Console.WriteLine(logo);
        }

        private void HandleDirectoryCommands(string command)
        {
            if (command.StartsWith("cd "))
            {
                string path = command.Substring(3);
                try
                {
                    if (path == "..")
                    {
                        Directory.SetCurrentDirectory("..");
                    }
                    else
                    {
                        Directory.SetCurrentDirectory(path);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else if (command == "ls" || command == "dir")
            {
                string[] dirs = Directory.GetDirectories(".");
                string[] files = Directory.GetFiles(".");

                Console.WriteLine("\nDirectories:");
                foreach (string dir in dirs)
                {
                    Console.WriteLine($"  [{Directory.GetLastWriteTime(dir):yyyy-MM-dd HH:mm}] {Path.GetFileName(dir)}/");
                }

                Console.WriteLine("\nFiles:");
                foreach (string file in files)
                {
                    Console.WriteLine($"  [{File.GetLastWriteTime(file):yyyy-MM-dd HH:mm}] {Path.GetFileName(file)}");
                }
            }
        }

        private void InitializeGames()
        {
            try
            {
                var gameManager = new GameManager();
                if (gameManager.Games.Count == 0)
                {
                    Console.WriteLine("Warning: No games loaded");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Game system initialization failed - {ex.Message}");
            }
        }
    }

    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AllocConsole();
    }
}
