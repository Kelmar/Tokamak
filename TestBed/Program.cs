using System;

using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace TestBed
{
    class Program
    {
        static void Main(string[] args)
        {
            var winSettings = new NativeWindowSettings
            {
                Size = new Vector2i(1920, 1080),
                Title = "OpenGL Test",
                APIVersion = new Version(4, 1)
            };

            using var window = new MainWindow(GameWindowSettings.Default, winSettings);
            window.Run();
        }
    }
}
