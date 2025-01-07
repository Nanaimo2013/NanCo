using System;
using System.IO;
using System.Media;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace NanCo.FileSystem
{
    public static class FileSystemManager
    {
        // Root directory for all NanCo files
        public static readonly string NanCoRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "NanCo"
        );

        // Subdirectories
        public static readonly string ProjectsDir = Path.Combine(NanCoRoot, "Projects");
        public static readonly string MediaDir = Path.Combine(NanCoRoot, "Media");
        public static readonly string ScriptsDir = Path.Combine(NanCoRoot, "Scripts");
        public static readonly string ConfigDir = Path.Combine(NanCoRoot, "Config");

        // Media subdirectories
        public static readonly string ImagesDir = Path.Combine(MediaDir, "Images");
        public static readonly string SoundsDir = Path.Combine(MediaDir, "Sounds");

        public static string TokenConfigPath => Path.Combine(ConfigDir, "tokens.json");

        public static void Initialize()
        {
            // Create all necessary directories
            Directory.CreateDirectory(NanCoRoot);
            Directory.CreateDirectory(ProjectsDir);
            Directory.CreateDirectory(MediaDir);
            Directory.CreateDirectory(ScriptsDir);
            Directory.CreateDirectory(ConfigDir);
            Directory.CreateDirectory(ImagesDir);
            Directory.CreateDirectory(SoundsDir);
        }

        public static readonly Dictionary<string, ConsoleColor> FileTypeColors = new()
        {
            {".ns", ConsoleColor.Cyan},      // NanCo Script
            {".nsp", ConsoleColor.Yellow},   // NanCo Project
            {".txt", ConsoleColor.White},    // Text file
            {".png", ConsoleColor.Green},    // Image
            {".jpg", ConsoleColor.Green},    // Image
            {".wav", ConsoleColor.Magenta},  // Audio
            {".mp3", ConsoleColor.Magenta},  // Audio
            {".conf", ConsoleColor.Gray},    // Config file
        };

        public static void ListFiles(string path = "")
        {
            string currentPath = string.IsNullOrEmpty(path) ? NanCoRoot : path;
            var info = new DirectoryInfo(currentPath);

            // Show current path
            Console.WriteLine($"\nCurrent Directory: {info.FullName}");
            Console.WriteLine("─".PadRight(Console.WindowWidth, '─'));

            // Show parent directory option if not in root
            if (info.FullName != NanCoRoot)
            {
                Console.WriteLine("│ [..] Parent Directory");
            }

            // List directories
            foreach (var dir in info.GetDirectories())
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"│ [{dir.Name}]");
                Console.ResetColor();
            }

            // List files with colors based on extension
            foreach (var file in info.GetFiles())
            {
                string ext = file.Extension.ToLower();
                Console.ForegroundColor = FileTypeColors.ContainsKey(ext) 
                    ? FileTypeColors[ext] 
                    : ConsoleColor.Gray;
                
                Console.WriteLine($"│ {file.Name}");
                Console.ResetColor();
            }
        }

        [SupportedOSPlatform("windows")]
        public static void ViewImage(string filename)
        {
            if (!OperatingSystem.IsWindows())
            {
                Console.WriteLine("Image viewing is only supported on Windows.");
                return;
            }

            string fullPath = Path.Combine(ImagesDir, filename);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine("Image file not found!");
                return;
            }

            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = fullPath,
                        UseShellExecute = true
                    }
                };
                process.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening image: {ex.Message}");
            }
        }

        [SupportedOSPlatform("windows")]
        public static void PlaySound(string filename)
        {
            if (!OperatingSystem.IsWindows())
            {
                Console.WriteLine("Sound playback is only supported on Windows.");
                return;
            }

            string fullPath = Path.Combine(SoundsDir, filename);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine("Sound file not found!");
                return;
            }

            try
            {
                using var player = new SoundPlayer(fullPath);
                player.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound: {ex.Message}");
            }
        }
    }
} 