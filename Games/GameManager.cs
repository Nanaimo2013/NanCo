using System;
using System.Collections.Generic;
using NanCo.Games;
using NanCo;

namespace NanCo.Games
{
    public class GameManager
    {
        public List<IGame> Games { get; private set; }
        private readonly LeaderboardService? leaderboardService;

        public GameManager()
        {
            Games = new List<IGame>();
            try
            {
                // Only initialize leaderboard if token is configured
                if (!string.IsNullOrEmpty(Config.GITHUB_TOKEN))
                {
                    leaderboardService = new LeaderboardService();
                }
                InitializeGames();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Leaderboard service initialization failed: {ex.Message}");
                InitializeGames();
            }
        }

        private void InitializeGames()
        {
            Games.Add(new SnakeGame(leaderboardService));
            Games.Add(new DinoGame(leaderboardService));
        }

        public void ListGames()
        {
            Console.WriteLine("\nAvailable Games:");
            Console.WriteLine("===============");
            
            for (int i = 0; i < Games.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Games[i].Name}");
                Console.WriteLine($"   {Games[i].Description}\n");
            }
        }

        public void StartGame(int index)
        {
            if (index >= 0 && index < Games.Count)
            {
                Console.Clear();
                Games[index].Start();
                Console.Clear();
            }
        }

        public void RegisterGame(string gameName)
        {
            var gameType = Type.GetType(gameName);
            if (gameType != null && typeof(IGame).IsAssignableFrom(gameType))
            {
                var game = (IGame)Activator.CreateInstance(gameType);
                Games.Add(game);
            }
        }

        public static readonly Dictionary<string, string> GameTemplates = new()
        {
            {"basic", @"using System;
using NanCo.Games;

public class NewGame : BaseGame
{
    public override string Name => ""GameName"";
    public override string Description => ""Game Description"";
    private Random random = new Random();

    public override void HandleInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Escape:
                Stop();
                break;
        }
    }

    public override void Update()
    {
        // Game logic here
    }

    public override void Render()
    {
        Console.Clear();
        DrawBorder();
        // Add rendering code here
    }
}"},
            {"arcade", @"using System;
using NanCo.Games;

public class ArcadeGame : BaseGame
{
    public override string Name => ""ArcadeName"";
    public override string Description => ""Arcade Style Game"";
    protected int Score { get; set; } = 0;
    protected int Level { get; set; } = 1;
    protected int Lives { get; set; } = 3;

    public override void HandleInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Escape:
                Stop();
                break;
        }
    }

    public override void Update()
    {
        if (Lives <= 0) Stop();
        // Add game logic here
    }

    public override void Render()
    {
        Console.Clear();
        DrawBorder();
        
        // Display game stats
        Console.SetCursorPosition(Width + 3, 1);
        Console.Write($""Score: {Score}"");
        Console.SetCursorPosition(Width + 3, 2);
        Console.Write($""Level: {Level}"");
        Console.SetCursorPosition(Width + 3, 3);
        Console.Write($""Lives: {Lives}"");
    }
}"}
        };
    }
}
