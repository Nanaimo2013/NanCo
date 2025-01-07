using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace NanCo.Games
{
    public class SnakeGame : BaseGame
    {
        private struct Position 
        {
            public int X { get; }
            public int Y { get; }

            public Position(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static bool operator ==(Position a, Position b) => 
                a.X == b.X && a.Y == b.Y;
            
            public static bool operator !=(Position a, Position b) => 
                !(a == b);

            public override bool Equals(object? obj)
            {
                if (obj is Position other)
                {
                    return X == other.X && Y == other.Y;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y);
            }
        }
        
        public override string Name => "Snake";
        public override string Description => "Classic snake game. Eat food to grow, avoid walls and yourself!";
        
        private List<Position> snake;
        private List<Position> foods;
        private Position direction;
        private Random random;
        private Position lastTail;
        private GameStats stats;
        private LeaderboardService? leaderboard;
        private DateTime startTime;
        private int gameSpeed = 100;
        private int timeElapsed = 0;
        private readonly int speedIncreaseInterval = 30;
        private readonly int maxFoodCount = 3;
        private readonly int foodSpawnChance = 35;
        private readonly Dictionary<string, ConsoleColor> colors = new()
        {
            {"border", ConsoleColor.Cyan},
            {"snake", ConsoleColor.Green},
            {"food", ConsoleColor.Red},
            {"bonus_food", ConsoleColor.Magenta},
            {"score", ConsoleColor.Yellow},
            {"stats", ConsoleColor.White},
            {"timer", ConsoleColor.DarkYellow}
        };

        public SnakeGame(LeaderboardService? leaderboardService = null)
        {
            random = new Random();
            snake = new List<Position>();
            foods = new List<Position>();
            direction = new Position(1, 0);
            stats = new GameStats("Snake", Environment.UserName);
            this.leaderboard = leaderboardService;
        }

        public override void Start()
        {
            stats = GameStats.LoadStats("Snake", Environment.UserName);
            startTime = DateTime.Now;
            stats.TimesPlayed++;
            timeElapsed = 0;
            gameSpeed = 100;

            // Initialize snake position
            snake.Clear();
            foods.Clear();
            snake.Add(new Position(Width / 2, Height / 2));
            direction = new Position(1, 0);
            
            // Initialize first food
            SpawnFood();
            
            DrawBorder();
            base.Start();
        }

        public override void HandleInput(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow when direction.Y != 1:
                    direction = new Position(0, -1);
                    break;
                case ConsoleKey.DownArrow when direction.Y != -1:
                    direction = new Position(0, 1);
                    break;
                case ConsoleKey.LeftArrow when direction.X != 1:
                    direction = new Position(-1, 0);
                    break;
                case ConsoleKey.RightArrow when direction.X != -1:
                    direction = new Position(1, 0);
                    break;
                case ConsoleKey.Escape:
                    Stop();
                    break;
            }
        }

        public override void Update()
        {
            // Update timer
            timeElapsed++;
            if (timeElapsed % (speedIncreaseInterval * 10) == 0) // Every 30 seconds
            {
                gameSpeed = Math.Max(50, gameSpeed - 10); // Speed up, minimum 50ms
            }

            // Chance to spawn additional food
            if (foods.Count < maxFoodCount && random.Next(100) < foodSpawnChance)
            {
                SpawnFood();
            }

            var head = snake.First();
            var newHead = new Position(head.X + direction.X, head.Y + direction.Y);

            // Check collisions
            if (newHead.X <= 0 || newHead.X >= Width || 
                newHead.Y <= 0 || newHead.Y >= Height ||
                snake.Contains(newHead))
            {
                Stop();
                return;
            }

            snake.Insert(0, newHead);
            lastTail = snake.Last();

            // Check if food was eaten
            var eatenFood = foods.FirstOrDefault(f => f == newHead);
            if (!eatenFood.Equals(default(Position)))
            {
                foods.Remove(eatenFood);
                if (foods.Count == 0) SpawnFood(); // Ensure at least one food
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }

            Thread.Sleep(gameSpeed);
        }

        public override void Render()
        {
            // Clear last tail
            Console.SetCursorPosition(lastTail.X, lastTail.Y);
            Console.Write(" ");
            
            // Draw snake
            Console.ForegroundColor = colors["snake"];
            foreach (var segment in snake)
            {
                Console.SetCursorPosition(segment.X, segment.Y);
                Console.Write("█");
            }
            
            // Draw all food items
            foreach (var food in foods)
            {
                Console.ForegroundColor = colors["food"];
                Console.SetCursorPosition(food.X, food.Y);
                Console.Write("♥");
            }
            
            // Draw stats
            Console.ForegroundColor = colors["stats"];
            Console.SetCursorPosition(Width + 3, 1);
            Console.Write($"Score: {snake.Count - 1}");
            Console.SetCursorPosition(Width + 3, 2);
            Console.Write($"High Score: {stats.HighScore}");
            Console.SetCursorPosition(Width + 3, 3);
            Console.Write($"Speed: {Math.Round((1000.0 / gameSpeed), 1)}x");
            Console.SetCursorPosition(Width + 3, 4);
            Console.Write($"Time: {timeElapsed / 10}s");
            
            Console.ResetColor();
        }

        private void SpawnFood()
        {
            Position newFood;
            do
            {
                newFood = new Position(random.Next(1, Width), random.Next(1, Height));
            } while (snake.Contains(newFood) || foods.Contains(newFood));
            
            foods.Add(newFood);
        }

        public override void Stop()
        {
            int finalScore = snake.Count - 1;
            stats.HighScore = Math.Max(stats.HighScore, finalScore);
            stats.TimePlayed += DateTime.Now - startTime;
            stats.LastPlayed = DateTime.Now;
            stats.SaveStats();
            
            // Update online leaderboard
            Task.Run(async () => 
            {
                if (leaderboard != null)
                {
                    await leaderboard.UpdateLeaderboard("Snake", Environment.UserName, finalScore);
                }
            }).Wait();

            base.Stop();
        }

        protected override void DrawBorder()
        {
            Console.ForegroundColor = colors["border"];
            base.DrawBorder();
            Console.ResetColor();
        }
    }
} 