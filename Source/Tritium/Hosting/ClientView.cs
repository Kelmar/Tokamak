using System;
using System.Threading;

using Silk.NET.Core.Contexts;
using Silk.NET.Maths;
using Silk.NET.Windowing;

using Tokamak.Core;

namespace Tokamak.Tritium.Hosting
{
    internal sealed class ClientView : IDisposable
    {
        private readonly IWindow m_silkWindow;
        private readonly IView m_silkView;

        private readonly IGameApp m_app;

        private bool m_shutdownCalled = false;

        public ClientView(IHostEnvironment hostEnvironment, IGameApp app)
        {
            m_app = app;

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

            m_silkView.Load += m_app.OnLoad;
            m_silkView.Update += m_app.OnUpdate;
            m_silkView.Render += m_app.OnRender;

            m_silkView.Closing += Shutdown;
        }

        private void CleanupEvents()
        {
            Shutdown();

            m_silkView.Closing -= Shutdown;

            m_silkView.Render -= m_app.OnRender;
            m_silkView.Update -= m_app.OnUpdate;
            m_silkView.Load -= m_app.OnLoad;
        }

        public void Dispose()
        {
            CleanupEvents();

            m_silkWindow?.Dispose();

            GC.SuppressFinalize(this);
        }

        public IGLContextSource Context => m_silkView;

        private void Shutdown()
        {
            if (m_shutdownCalled)
                return;

            // Ensure the shutdown code gets called.
            try
            {
                m_app.OnShutdown();
            }
            catch
            {
                /*
                 * BUG?
                 * For now we discard to prevent exception in Dispose() method.
                 * We should probably log this later though.
                 */
            }
            finally
            {
                m_shutdownCalled = true;
            }
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
