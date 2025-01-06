using System;
using System.Threading;

namespace NanCo.Games
{
    public class DinoGame
    {
        private const int WIDTH = 70;
        private const int HEIGHT = 10;
        private int dinoPos = 1;
        private bool isJumping = false;
        private int jumpHeight = 0;
        private int obstaclePos = WIDTH - 1;
        
        public void Start()
        {
            Console.Clear();
            Console.CursorVisible = false;
            
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Spacebar && !isJumping)
                    {
                        isJumping = true;
                    }
                }

                UpdateGame();
                DrawGame();
                Thread.Sleep(50);
            }
        }

        private void UpdateGame()
        {
            if (isJumping)
            {
                if (jumpHeight < 4)
                    jumpHeight++;
                else
                    isJumping = false;
            }
            else if (jumpHeight > 0)
            {
                jumpHeight--;
            }

            obstaclePos--;
            if (obstaclePos < 0)
                obstaclePos = WIDTH - 1;
        }

        private void DrawGame()
        {
            Console.SetCursorPosition(0, 0);
            
            // Draw dino and obstacles
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    if (x == dinoPos && y == HEIGHT - 2 - jumpHeight)
                        Console.Write("D");
                    else if (x == obstaclePos && y == HEIGHT - 2)
                        Console.Write("|");
                    else
                        Console.Write(" ");
                }
                Console.WriteLine();
            }
            
            // Draw ground
            Console.WriteLine(new string('_', WIDTH));
        }
    }
} 