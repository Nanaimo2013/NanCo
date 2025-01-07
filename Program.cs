using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NanCo
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(int dwProcessId);

        private const int ATTACH_PARENT_PROCESS = -1;

        static void Main(string[] args)
        {
            try
            {
                // Detach from any existing console
                FreeConsole();
                
                // Create a new console
                if (!AllocConsole())
                {
                    // If allocation fails, try to attach to parent
                    if (!AttachConsole(ATTACH_PARENT_PROCESS))
                    {
                        Console.WriteLine("Failed to initialize console.");
                        return;
                    }
                }

                // Give the console time to initialize
                Thread.Sleep(100);

                // Start the application
                Boot.Start();
                var terminal = new Terminal();
                terminal.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                try
                {
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey(true);
                }
                catch
                {
                    // Ignore if console input is not available
                }
            }
        }
    }
}