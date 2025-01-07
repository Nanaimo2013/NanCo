using System;
using System.IO;

namespace NanCo
{
    public static class Config
    {
        // GitHub Configuration
        public const string GITHUB_TOKEN = "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";
        public const string GITHUB_OWNER = "Nanaimo2013";
        public const string GITHUB_REPO = "NanCo";
        public const string GIST_ID = "cecaa226a1d3fd55754c96a9e760c5aa";

        // Version Configuration
        public const string DEFAULT_VERSION = "v1.12.4-beta";
        
        // Application Paths
        public static readonly string AppDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NanCo"
        );
    }
} 