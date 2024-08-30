using System;
using System.Threading;

using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Tokamak.Core.Implementation
{
    internal sealed class ClientView : IDisposable
    {
        private readonly IWindow m_silkWindow;
        private readonly IView m_silkView;

        public ClientView(IHostEnvironment hostEnvironment)
        {
            if (Window.IsViewOnly)
            {
                m_silkWindow = null;
                m_silkView = Window.GetView();
            }
            else
            {
                m_silkWindow = InitWindowedMode(hostEnvironment);
                m_silkView = m_silkWindow;
            }
        }

        public void Dispose()
        {
            m_silkWindow?.Dispose();
            GC.SuppressFinalize(this);
        }

        private IWindow InitWindowedMode(IHostEnvironment hostEnvironment)
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(1920, 1080);

            options.Title = hostEnvironment.ApplicationName;

            // TODO: Pull from driver
            options.VSync = false;
            options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(4, 1));

            return Window.Create(options);
        }

        public void Run(CancellationToken cancellationToken = default)
        {
            m_silkView.Run();
        }
    }
}
