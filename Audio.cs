// Audio.cs
using System;
using System.Media;
using System.Runtime.Versioning;
using System.Threading;

namespace NanCo
{
    static class Audio
    {
        private static readonly Random random = new Random();

        [SupportedOSPlatform("windows")]
        public static void PlaySound(string filePath)
        {
            if (!OperatingSystem.IsWindows()) return;
            
            try
            {
                using (var player = new SoundPlayer(filePath))
                {
                    player.Play();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound: {ex.Message}");
            }
        }

        public static void Noise(string kind)
        {
            try
            {
                switch (kind)
                {
                    case "boot":
                        PlaySound("audios/boot.wav");
                        break;
                    case "hgboot":
                        PlaySound("audios/hgboot.wav");
                        break;
                    case "keys":
                        int noise = random.Next(1, 10);
                        PlaySound($"audios/{noise}.wav");
                        break;
                    default:
                        Console.WriteLine("Invalid audio kind.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound: {ex.Message}");
            }
        }

        public static void PlayBootSound()
        {
            Noise("boot");
        }

        public static void PlayHQBootSound()
        {
            Noise("hgboot");
        }

        public static void PlaySuccess()
        {
            if (OperatingSystem.IsWindows())
            {
                PlayBeep(1000, 100);
                Thread.Sleep(50);
                PlayBeep(1500, 100);
            }
        }

        public static void PlayError()
        {
            if (OperatingSystem.IsWindows())
            {
                PlayBeep(200, 300);
            }
        }

        public static void PlayWarning()
        {
            if (OperatingSystem.IsWindows())
            {
                PlayBeep(800, 100);
                Thread.Sleep(50);
                PlayBeep(800, 100);
            }
        }

        public static void PlayKeyPress()
        {
            if (OperatingSystem.IsWindows())
            {
                PlayBeep(random.Next(800, 1200), 10);
            }
        }

        public static void PlayLogin()
        {
            if (OperatingSystem.IsWindows())
            {
                PlayBeep(1200, 100);
                Thread.Sleep(30);
                PlayBeep(1500, 100);
                Thread.Sleep(30);
                PlayBeep(2000, 150);
            }
        }

        public static void PlayLogout()
        {
            if (OperatingSystem.IsWindows())
            {
                PlayBeep(2000, 100);
                Thread.Sleep(30);
                PlayBeep(1500, 100);
                Thread.Sleep(30);
                PlayBeep(1200, 150);
            }
        }

        public static void PlayBeep(int frequency, int duration)
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    Console.Beep(frequency, duration);
                }
                else
                {
                    // Fallback for non-Windows systems
                    Thread.Sleep(duration);
                }
            }
            catch
            {
                Thread.Sleep(duration);
            }
        }
    }
}



