using System;
using System.Threading;

namespace NanCo.Games
{
    public abstract class BaseGame : IGame
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public bool IsRunning { get; protected set; }
        protected int Width { get; } = 40;
        protected int Height { get; } = 20;
        
        public virtual void Start()
        {
            IsRunning = true;
            Console.CursorVisible = false;
            
            // Initial screen setup
            Console.Clear();
            DrawBorder();
            InitializeDisplay();
            
            while (IsRunning)
            {
                if (Console.KeyAvailable)
                {
                    HandleInput(Console.ReadKey(true));
                }
                
                Update();
                Render();
                Thread.Sleep(100);
            }
            
            Console.CursorVisible = true;
        }

        public virtual void Stop()
        {
            IsRunning = false;
        }

        public abstract void HandleInput(ConsoleKeyInfo keyInfo);
        public abstract void Update();
        public abstract void Render();

        protected virtual void DrawBorder()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write('╔' + new string('═', Width) + '╗');
            
            for (int i = 1; i <= Height; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write('║');
                Console.SetCursorPosition(Width + 1, i);
                Console.Write('║');
            }
            
            Console.SetCursorPosition(0, Height + 1);
            Console.Write('╚' + new string('═', Width) + '╝');
        }

        protected virtual void InitializeDisplay()
        {
            // Draw static UI elements
            Console.SetCursorPosition(Width + 3, 1);
            Console.Write("Score: 0");
        }
    }
} 