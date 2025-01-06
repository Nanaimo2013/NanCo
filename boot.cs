using System;
using System.Threading;

namespace NanCo
{
    public static class Boot
    {
        private static readonly Random random = new Random();

        public static void Start()
        {
            // Set console colors
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Clear();

            Effects.TypewriterEffect("INITIALIZING TERMINAL...");
            Effects.LoadingBar(20);
            Console.WriteLine(" [OK]");

            if (OperatingSystem.IsWindows())
            {
                try
                {
                    Effects.TypewriterEffect("CONFIGURING DISPLAY...");
                    Effects.LoadingBar(10);
                    Console.SetWindowSize(120, 40);
                    Console.WriteLine(" [OK]");
                }
                catch
                {
                    Console.WriteLine(" [FAILED]");
                }
            }
            Console.Title = "NanCo Terminal";
            
            // Initial POST sequence
            PowerOnSelfTest();
            
            // Play startup sound
            Effects.TypewriterEffect("INITIALIZING AUDIO SYSTEM...");
            Effects.LoadingBar(15);
            Audio.PlayBootSound();
            Console.WriteLine(" [OK]");
            
            // Initial screen flicker effect
            Effects.TypewriterEffect("PERFORMING DISPLAY TEST...");
            Effects.LoadingBar(10);
            Effects.ScreenFlicker();
            Console.WriteLine(" [OK]");
            
            DisplayNanCoLogo();
            
            // Detailed boot sequence
            RunDetailedBootSequence();
            
            // Initialize systems
            InitializeSystems();

            // Final boot message
            Effects.TypewriterEffect("\nVERIFYING BOOT SEQUENCE...");
            Effects.LoadingBar(30);
            Console.WriteLine(" [VERIFIED]");
            
            Effects.TypewriterEffect("BOOT SEQUENCE COMPLETE - TERMINAL READY");
            Effects.LoadingBar(40);
            Console.WriteLine("\n");
        }

        private static void PowerOnSelfTest()
        {
            string[] postMessages = {
                "PERFORMING POST...",
                "CHECKING CPU STATUS...",
                "INITIALIZING MEMORY...",
                "DETECTING HARDWARE...",
                "VERIFYING SYSTEM INTEGRITY...",
                "LOADING SYSTEM PARAMETERS..."
            };

            foreach (var msg in postMessages)
            {
                Effects.TypewriterEffect(msg);
                Effects.LoadingBar(random.Next(10, 25));
                Console.WriteLine(" [OK]");
                Audio.PlayBeep(800, 30);
                Thread.Sleep(200);
            }
            Console.WriteLine();
        }

        private static void DisplayNanCoLogo()
        {
            Effects.TypewriterEffect("LOADING SYSTEM IDENTITY...");
            Effects.LoadingBar(20);
            Console.WriteLine(" [OK]\n");

            string nancoLogo = @"
    ███╗   ██╗ █████╗ ███╗   ██╗ ██████╗ ██████╗ 
    ████╗  ██║██╔══██╗████╗  ██║██╔════╝██╔═══██╗
    ██╔██╗ ██║███████║██╔██╗ ██║██║     ██║   ██║
    ██║╚██╗██║██╔══██║██║╚██╗██║██║     ██║   ██║
    ██║ ╚████║██║  ██║██║ ╚████║╚██████╗╚██████╔╝
    ╚═╝  ╚═══╝╚═╝  ╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚═════╝ 
                                                  
     ╔═══════════════════════════════════════════╗
     ║      NanS Studio Proprietary Systems      ║
     ║         (c) 2024 All Rights Reserved      ║
     ╚═══════════════════════════════════════════╝
";
            
            foreach (var line in nancoLogo.Split('\n'))
            {
                Console.WriteLine(line);
                Audio.PlayBeep(random.Next(300, 800), 10);
                Thread.Sleep(50);
            }

            Effects.TypewriterEffect("VERIFYING SYSTEM SIGNATURE...");
            Effects.LoadingBar(25);
            Console.WriteLine(" [AUTHENTICATED]");
            Thread.Sleep(1000);
            Console.WriteLine();
        }

        private static void RunDetailedBootSequence()
        {
            string[] diagnostics = {
                "Initializing BIOS",
                "Loading Memory Management",
                "Checking Hardware Components",
                "Loading System Kernel",
                "Configuring System Parameters"
            };

            Console.WriteLine("RUNNING SYSTEM DIAGNOSTICS:");
            Console.WriteLine("═".PadRight(50, '═'));

            foreach (var line in diagnostics)
            {
                Effects.TypewriterEffect($">> {line}...");
                Effects.LoadingBar(random.Next(15, 30));
                Console.WriteLine(" [OK]");
                Audio.PlayBeep(1200, 40);
                Thread.Sleep(300);
            }

            // Technical readouts
            string[] technicalInfo = {
                "MEMORY TEST",
                "RAM VERIFICATION",
                "STORAGE SCAN",
                "HOLOTAPE CHECK",
                "TERMINAL SERVICES",
                "FILE SYSTEM CHECK"
            };

            Console.WriteLine("\nSYSTEM DIAGNOSTIC REPORT:");
            Console.WriteLine("═".PadRight(50, '═'));
            
            foreach (var info in technicalInfo)
            {
                Effects.TypewriterEffect($">> {info}...");
                Effects.LoadingBar(random.Next(10, 20));
                Console.WriteLine(" [VERIFIED]");
                Audio.PlayBeep(random.Next(500, 2000), 50);
                Thread.Sleep(300);
            }
        }

        private static void InitializeSystems()
        {
            string[] systems = {
                "Terminal Interface",
                "File System",
                "Command Processor",
                "IDE Subsystem",
                "Audio Subsystem",
                "Security Protocol",
                "Network Stack",
                "System Services"
            };

            Console.WriteLine("\nINITIALIZING CORE SYSTEMS:");
            Console.WriteLine("═".PadRight(50, '═'));
            
            foreach (var system in systems)
            {
                Effects.TypewriterEffect($"Loading {system}...");
                Effects.LoadingBar(random.Next(15, 25));
                Console.WriteLine(" [ONLINE]");
                Audio.PlayBeep(1000, 30);
                Thread.Sleep(200);
            }

            // Final system check
            Console.WriteLine("\nPERFORMING FINAL SYSTEM CHECK:");
            Effects.LoadingBar(40);
            Console.WriteLine(" [COMPLETE]");
            Audio.PlayBeep(2000, 100);

            Effects.TypewriterEffect("VALIDATING SYSTEM STATE...");
            Effects.LoadingBar(30);
            Console.WriteLine(" [OPTIMAL]");
        }
    }
}

