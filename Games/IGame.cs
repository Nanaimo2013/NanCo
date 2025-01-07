using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanCo.Games
{
    public interface IGame
    {
        string Name { get; }
        string Description { get; }
        bool IsRunning { get; }
        void Start();
        void Stop();
        void HandleInput(ConsoleKeyInfo keyInfo);
        void Update();
        void Render();
    }
} 