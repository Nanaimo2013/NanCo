using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Linq;
using NanCo;

namespace NanCo
{
    public class UpdateManager
    {
        private readonly HttpClient client;
        private const string REPO_OWNER = Config.GITHUB_OWNER;
        private const string REPO_NAME = Config.GITHUB_REPO;
        private readonly string currentVersion;

        public UpdateManager()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "NanCo-Updater");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            currentVersion = VersionManager.GetVersion();
        }

        public async Task CheckForUpdates()
        {
            try
            {
                var releases = await client.GetStringAsync(
                    $"https://api.github.com/repos/{REPO_OWNER}/{REPO_NAME}/releases"
                );

                var releaseData = JsonSerializer.Deserialize<List<Release>>(releases);
                var latestRelease = releaseData?.FirstOrDefault();

                if (latestRelease?.tag_name != null && IsNewerVersion(latestRelease.tag_name, currentVersion))
                {
                    Console.WriteLine($"\nNew version available: {latestRelease.tag_name}");
                    if (latestRelease.zipball_url != null)
                    {
                        await PerformUpdate(latestRelease.zipball_url);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update check failed: {ex.Message}");
            }
        }

        private bool IsNewerVersion(string newVersion, string currentVersion)
        {
            // Remove 'v' prefix if present
            newVersion = newVersion.TrimStart('v');
            currentVersion = currentVersion.TrimStart('v');

            // Split version into parts (major.minor.patch-suffix)
            var newParts = newVersion.Split(new[] { '.', '-' });
            var currentParts = currentVersion.Split(new[] { '.', '-' });

            // Compare numeric parts
            for (int i = 0; i < Math.Min(3, Math.Min(newParts.Length, currentParts.Length)); i++)
            {
                if (int.TryParse(newParts[i], out int newNum) && 
                    int.TryParse(currentParts[i], out int currentNum))
                {
                    if (newNum != currentNum)
                        return newNum > currentNum;
                }
            }

            // If numeric parts are equal, compare suffixes (beta < rc < release)
            var newSuffix = newParts.Length > 3 ? newParts[3] : "";
            var currentSuffix = currentParts.Length > 3 ? currentParts[3] : "";

            return CompareSuffixes(newSuffix, currentSuffix);
        }

        private bool CompareSuffixes(string newSuffix, string currentSuffix)
        {
            var suffixPriority = new Dictionary<string, int>
            {
                { "", 3 },        // release
                { "rc", 2 },      // release candidate
                { "beta", 1 },    // beta
                { "alpha", 0 }    // alpha
            };

            int newPriority = suffixPriority.GetValueOrDefault(newSuffix.ToLower(), -1);
            int currentPriority = suffixPriority.GetValueOrDefault(currentSuffix.ToLower(), -1);

            return newPriority > currentPriority;
        }

        private async Task PerformUpdate(string zipUrl)
        {
            var tempPath = Path.GetTempPath();
            var zipPath = Path.Combine(tempPath, "nanco_update.zip");
            var extractPath = Path.Combine(tempPath, "nanco_update");

            // Download update
            using (var response = await client.GetStreamAsync(zipUrl))
            using (var fileStream = File.Create(zipPath))
            {
                await response.CopyToAsync(fileStream);
            }

            // Create update script
            var scriptPath = Path.Combine(tempPath, "update.bat");
            File.WriteAllText(scriptPath, @$"
@echo off
timeout /t 2 /nobreak
tar -xf ""{zipPath}"" -C ""{extractPath}""
xcopy /E /Y ""{extractPath}\*"" ""{AppDomain.CurrentDomain.BaseDirectory}""
del ""{zipPath}""
rmdir /S /Q ""{extractPath}""
del ""%~f0""
start """" ""{Process.GetCurrentProcess().MainModule.FileName}""
");

            // Start update process and exit current application
            Process.Start(scriptPath);
            Environment.Exit(0);
        }

        private class Release
        {
            public string? tag_name { get; set; }
            public string? zipball_url { get; set; }
            public string? body { get; set; }
        }
    }
} 