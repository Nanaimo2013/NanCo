using System;
using System.IO;

namespace NanCo
{
    public static class VersionManager
    {
        private const string VERSION_FILE_NAME = "version.txt";
        private static readonly string VERSION_FILE_PATH = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, 
            VERSION_FILE_NAME
        );
        private const string DEFAULT_VERSION = "v1.12.4-beta";

        public static string GetVersion()
        {
            try
            {
                if (!File.Exists(VERSION_FILE_PATH))
                {
                    File.WriteAllText(VERSION_FILE_PATH, DEFAULT_VERSION);
                }
                return File.ReadAllText(VERSION_FILE_PATH).Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading version: {ex.Message}");
                return DEFAULT_VERSION;
            }
        }
    }
} 