using System;
using System.Collections.Generic;
using System.Threading;

namespace NanCo.Games
{
    public class DinoGame : BaseGame
    {
        private readonly LeaderboardService? leaderboard;

        public DinoGame(LeaderboardService? leaderboardService = null)
        {
            this.leaderboard = leaderboardService;
        }

        public override string Name => "Dino";
        public override string Description => "Jump over obstacles, avoid getting hit!";
        
        private int playerY;
        private bool isJumping;
        private int jumpHeight;
        private int score;
        private List<int> obstaclePositions;
        private Random random;
        private int gameSpeed;
        private int lastScore = 0;
        private List<int> lastObstaclePositions = new List<int>();

        public DinoGame()
        {
            random = new Random();
            obstaclePositions = new List<int>();
            Reset();
        }

        private void Reset()
        {
            playerY = Height - 3;
            isJumping = false;
            jumpHeight = 0;
            score = 0;
            gameSpeed = 100;
            obstaclePositions.Clear();
            obstaclePositions.Add(Width - 5);
        }

        public override void HandleInput(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Spacebar:
                case ConsoleKey.UpArrow:
                    if (!isJumping)
                    {
                        isJumping = true;
                        jumpHeight = 6;
                    }
                    break;
                case ConsoleKey.Escape:
                    Stop();
                    break;
            }
        }

        public override void Update()
        {
            // Update jump
            if (isJumping)
            {
                if (jumpHeight > 0)
                {
                    playerY--;
                    jumpHeight--;
                }
                else
                {
                    if (playerY < Height - 3)
                    {
                        playerY++;
                    }
                    else
                    {
                        isJumping = false;
                    }
                }
            }

            // Update obstacles
            for (int i = obstaclePositions.Count - 1; i >= 0; i--)
            {
                obstaclePositions[i]--;
                if (obstaclePositions[i] < 0)
                {
                    obstaclePositions.RemoveAt(i);
                    score++;
                    if (score % 5 == 0 && gameSpeed > 50)
                    {
                        gameSpeed -= 5;
                    }
                }
            }

            // Add new obstacles
            if (obstaclePositions.Count < 3 && random.Next(20) == 0)
            {
                obstaclePositions.Add(Width - 5);
            }

            // Check collisions
            foreach (var obstacleX in obstaclePositions)
            {
                if (obstacleX == 10 && playerY >= Height - 4)
                {
                    Stop();
                    Console.SetCursorPosition(Width / 2 - 5, Height / 2);
                    Console.WriteLine("Game Over!");
                    Thread.Sleep(2000);
                }
            }
        }

        public override void Render()
        {
            // Clear old obstacle positions
            foreach (var pos in lastObstaclePositions)
            {
                Console.SetCursorPosition(pos, Height - 3);
                Console.Write(" ");
            }
            
            // Draw new obstacle positions
            foreach (var pos in obstaclePositions)
            {
                Console.SetCursorPosition(pos, Height - 3);
                Console.Write("▲");
            }
            
            // Update player position
            Console.SetCursorPosition(10, playerY);
            Console.Write("█");
            Console.SetCursorPosition(10, playerY + 1);
            Console.Write(" ");
            
            // Update score if changed
            if (score != lastScore)
            {
                Console.SetCursorPosition(Width + 10, 1);
                Console.Write(score.ToString().PadLeft(5));
                lastScore = score;
            }
            
            // Store current obstacle positions for next render
            lastObstaclePositions = new List<int>(obstaclePositions);
        }
    }
} 