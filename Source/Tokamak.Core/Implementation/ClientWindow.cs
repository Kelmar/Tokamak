using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Silk.NET.Maths;
using Silk.NET.Windowing;

using Stashbox;

namespace Tokamak.Core.Implementation
{
    internal sealed class ClientWindow : IDisposable
    {

        private readonly IWindow m_silkWindow;

        public ClientWindow()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(1920, 1080);

            // TODO: Pull from config
            options.Title = "Tokamak";

            // TODO: Pull from driver
            options.VSync = false;
            options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(4, 1));

            m_silkWindow = Window.Create(options);
        }

        public void Dispose()
        {
            m_silkWindow.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Run()
        {
            m_silkWindow.Run();
        }
    }
}
