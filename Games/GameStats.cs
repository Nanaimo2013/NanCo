using System;
using System.IO;
using System.Text.Json;

public class GameStats
{
    public string GameName { get; set; }
    public string PlayerName { get; set; }
    public int HighScore { get; set; }
    public int TimesPlayed { get; set; }
    public TimeSpan TimePlayed { get; set; }
    public DateTime LastPlayed { get; set; }

    public GameStats(string gameName, string playerName)
    {
        GameName = gameName;
        PlayerName = playerName;
        HighScore = 0;
        TimesPlayed = 0;
        TimePlayed = TimeSpan.Zero;
        LastPlayed = DateTime.Now;
    }

    public void SaveStats()
    {
        string statsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NanCo", "GameStats", $"{PlayerName}_{GameName}.json"
        );
        Directory.CreateDirectory(Path.GetDirectoryName(statsPath));
        File.WriteAllText(statsPath, System.Text.Json.JsonSerializer.Serialize(this));
    }

    public static GameStats LoadStats(string gameName, string playerName)
    {
        string statsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NanCo", "GameStats", $"{playerName}_{gameName}.json"
        );
        
        if (File.Exists(statsPath))
        {
            return System.Text.Json.JsonSerializer.Deserialize<GameStats>(File.ReadAllText(statsPath));
        }
        return new GameStats(gameName, playerName);
    }
} 