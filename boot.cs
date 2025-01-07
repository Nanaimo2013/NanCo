using System;
using System.IO;
using System.Threading;
using NanCo.FileSystem;
using NanCo.Games;
using NanCo.Language;
using System.Threading.Tasks;

namespace NanCo
{
    public static class Boot
    {
        private static bool allTestsPassed = true;
        private static string? currentVersion;

        public static async Task Initialize()
        {
            currentVersion = GetVersion();
            Console.WriteLine($"Current Version: {currentVersion}");
            
            try
            {
                var updater = new UpdateManager();
                await updater.CheckForUpdates();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update check failed: {ex.Message}");
            }
        }

        public static void Start()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Clear();

            DisplayBootHeader();
            
            if (!ConfirmSystemTest())
            {
                Effects.TypewriterEffect("\nSystem test skipped. Starting NanCo...");
                Thread.Sleep(1000);
                return;
            }
            
            RunSystemTests();

            if (allTestsPassed)
            {
                Effects.TypewriterEffect("\nBOOT SEQUENCE COMPLETE - ALL TESTS PASSED");
                Effects.LoadingBar(20);
                Console.WriteLine("\n");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Effects.TypewriterEffect("\nWARNING: Some tests failed. System may be unstable.");
                if (!ConfirmContinue())
                {
                    Environment.Exit(1);
                }
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
        }

        private static bool ConfirmSystemTest()
        {
            Console.Write("\nRun system diagnostic tests? (Y/N): ");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Y)
                {
                    Console.WriteLine("Y");
                    return true;
                }
                if (key.Key == ConsoleKey.N)
                {
                    Console.WriteLine("N");
                    return false;
                }
            }
        }

        private static bool ConfirmContinue()
        {
            Console.Write("\nContinue anyway? (Y/N): ");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Y)
                {
                    Console.WriteLine("Y");
                    return true;
                }
                if (key.Key == ConsoleKey.N)
                {
                    Console.WriteLine("N");
                    return false;
                }
            }
        }

        private static string GetVersion()
        {
            return VersionManager.GetVersion();
        }

        private static void RunSystemTests()
        {
            // Version Check
            RunTest("Version Check", () => {
                string version = GetVersion();
                return version.StartsWith("v") && version.Contains("beta");
            });

            // File System Check
            RunTest("File System Check", () => {
                Console.WriteLine("\nChecking critical directories...");
                var dirs = new[] {
                    FileSystemManager.ProjectsDir,
                    FileSystemManager.ScriptsDir,
                    FileSystemManager.ConfigDir
                };

                foreach (var dir in dirs)
                {
                    Console.WriteLine($"Verifying: {Path.GetFileName(dir)}");
                    Directory.CreateDirectory(dir);
                    if (!Directory.Exists(dir)) return false;
                }
                return true;
            });

            // Game System Check
            RunTest("Game System Check", () => {
                Console.WriteLine("\nVerifying game system...");
                try
                {
                    var gameManager = new GameManager();
                    var gameCount = gameManager.Games.Count;
                    Console.WriteLine($"Found {gameCount} games");
                    return gameCount >= 2; // At least Snake and Dino games
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: {ex.Message}");
                    return true; // Continue even if leaderboard is unavailable
                }
            });

            // Script Engine Check
            RunTest("Script Engine Check", () => {
                Console.WriteLine("\nTesting script engine...");
                var testScript = Path.Combine(FileSystemManager.ScriptsDir, "test.ns");
                try {
                    File.WriteAllText(testScript, "print \"test\"");
                    var runner = new NanScriptRunner(testScript);
                    runner.Execute();
                    File.Delete(testScript);
                    return true;
                }
                catch (Exception ex) {
                    Console.WriteLine($"Script test failed: {ex.Message}");
                    return false;
                }
            });
        }

        private static void RunTest(string testName, Func<bool> test)
        {
            Effects.TypewriterEffect($"Testing {testName}...");
            Effects.LoadingBar(10);
            
            bool passed = false;
            try {
                passed = test();
            }
            catch (Exception ex) {
                Console.WriteLine($" [FAILED] - {ex.Message}");
                allTestsPassed = false;
                return;
            }

            if (passed) {
                Console.WriteLine(" [OK]");
                Audio.PlayBeep(1000, 30);
            }
            else {
                Console.WriteLine(" [FAILED]");
                Audio.PlayBeep(500, 100);
                allTestsPassed = false;
            }
        }

        private static void DisplayBootHeader()
        {
            string header = @"
    ███╗   ██╗ █████╗ ███╗   ██╗ ██████╗ ██████╗ 
    ████╗  ██║██╔══██╗████╗  ██║██╔════╝██╔═══██╗
    ██╔██╗ ██║███████║██╔██╗ ██║██║     ██║   ██║
    ██║╚██╗██║██╔══██║██║╚██╗██║██║     ██║   ██║
    ██║ ╚████║██║  ██║██║ ╚████║╚██████╗╚██████╔╝
    ╚═╝  ╚═══╝╚═╝  ╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚═════╝ 
    
     ╔═══════════════════════════════════════════╗
     ║           System Diagnostic Test          ║
     ╚═══════════════════════════════════════════╝";

            foreach (var line in header.Split('\n'))
            {
                Console.WriteLine(line);
                Thread.Sleep(50);
            }
        }
    }
}

