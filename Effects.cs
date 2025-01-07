using System;
using System.Threading;
using System.Linq;

namespace NanCo
{
    public static class Effects
    {
        private static readonly Random random = new Random();

        // Define colors
        private static readonly ConsoleColor RobCoGreen = ConsoleColor.DarkGreen;
        private static readonly ConsoleColor RobCoBlack = ConsoleColor.Black;
        private static readonly ConsoleColor LoadingBarColor = ConsoleColor.Green;

        public static void SetBackgroundColor(ConsoleColor color)
        {
            try
            {
                Console.BackgroundColor = color;
            }
            catch
            {
                // Ignore color setting errors
            }
        }

        public static void SetTextColor(ConsoleColor color)
        {
            try
            {
                Console.ForegroundColor = color;
            }
            catch
            {
                // Ignore color setting errors
            }
        }

        public static void SetFont(string fontName)
        {
            // Not supported in .NET Core console applications
        }

        private static void FadeInOut()
        {
            try
            {
                for (int i = 0; i <= 10; i++)
                {
                    Console.WriteLine($"color 0{i}");
                    Thread.Sleep(100);
                }

                for (int i = 10; i >= 0; i--)
                {
                    Console.WriteLine($"color 0{i}");
                    Thread.Sleep(100);
                }
            }
            catch
            {
                // Ignore fade effects errors
            }
        }

        private static void LoadingLog()
        {
            try
            {
                string loadingText = "Loading...";
                for (int i = 0; i < loadingText.Length; i++)
                {
                    Console.Write(loadingText[i]);
                    Thread.Sleep(100);
                }
                Console.WriteLine();
            }
            catch
            {
                // Ignore loading log errors
            }
        }

        public static void FalloutConsoleWriteLine(string text)
        {
            try
            {
                foreach (char c in text)
                {
                    Console.Write(c);
                    Thread.Sleep(5);
                }
                Console.WriteLine();
            }
            catch
            {
                // Fallback to normal write if effect fails
                Console.WriteLine(text);
            }
        }

        public static void TypingEffect(string text)
        {
            try
            {
                foreach (char c in text)
                {
                    Console.Write(c);
                    Thread.Sleep(1);
                }
                Console.WriteLine();
            }
            catch
            {
                // Fallback to normal write if effect fails
                Console.WriteLine(text);
            }
        }

        public static void SetRobCoBackground()
        {
            SetBackgroundColor(RobCoBlack);
        }

        public static void SetRobCoText()
        {
            SetTextColor(RobCoGreen);
        }

        public static void ScreenFlicker()
        {
            try
            {
                int width = Console.WindowWidth;
                int height = Console.WindowHeight;
                string blank = new string(' ', width);

                // Save current cursor position
                int originalLeft = Console.CursorLeft;
                int originalTop = Console.CursorTop;

                // Flicker effect
                for (int i = 0; i < 3; i++)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.ForegroundColor = ConsoleColor.Black;
                    
                    // Fill entire screen
                    Console.SetCursorPosition(0, 0);
                    for (int j = 0; j < height; j++)
                    {
                        Console.WriteLine(blank);
                    }
                    
                    Thread.Sleep(50);

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    
                    // Fill entire screen
                    Console.SetCursorPosition(0, 0);
                    for (int j = 0; j < height; j++)
                    {
                        Console.WriteLine(blank);
                    }
                    
                    Thread.Sleep(50);
                }

                // Restore cursor position
                Console.SetCursorPosition(originalLeft, originalTop);
            }
            catch
            {
                // Fallback if screen operations fail
                Thread.Sleep(300);
            }
        }

        public static void TypewriterEffect(string text)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                if (char.IsWhiteSpace(c))
                {
                    Thread.Sleep(random.Next(10, 20));
                }
                else
                {
                    Thread.Sleep(random.Next(5, 10));
                }
            }
        }

        public static void LoadingBar(int duration)
        {
            Console.Write("[");
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = LoadingBarColor;
            
            for (int i = 0; i < 20; i++)
            {
                Thread.Sleep(duration);
                Console.Write("█");
            }
            
            Console.ForegroundColor = originalColor;
            Console.Write("]");
        }

        public static void ScreenFlash()
        {
            // Removed the flashing effect completely
            Thread.Sleep(1);  // Small pause instead
        }
    }
}



