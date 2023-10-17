using System;

using Silk.NET.Windowing;

namespace TestBed
{
    class Program
    {
        static void Main(string[] args)
        {
            using var window = new MainWindow();

            window.Run();
        }
    }
}
