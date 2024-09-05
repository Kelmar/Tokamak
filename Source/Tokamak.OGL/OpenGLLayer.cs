using System;

using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

using Tokamak.Core;
using Tokamak.Mathematics;

using Tokamak.Tritium.APIs;

using TokPixelFormat = Tokamak.Formats.PixelFormat;

namespace Tokamak.OGL
{
    internal class OpenGLLayer : IAPILayer, ITick
    {
        private readonly IWindow m_window;
        private readonly IView m_view;

        private readonly IGameLifetime m_gameLifetime;

        private TextureObject m_whiteTexture = null;

        public OpenGLLayer(IHostEnvironment hostEnvironment, IGameLifetime gameLifetime)
        {
            m_gameLifetime = gameLifetime;

            if (Window.IsViewOnly)
            {
                m_window = null;
                m_view = Window.GetView();
            }
            else
            {
                m_window = InitWindowedMode(hostEnvironment);
                m_view = m_window;
            }

            InitEvents();

            m_view.Initialize();

            // Ensure OS events are taken care of first thing.
            m_gameLifetime.AddTick(this, TickPriority.Highest);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (GL != null)
                {
                    m_whiteTexture?.Dispose();
                    GL.Dispose();
                }

                CleanupEvents();

                m_window?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public GL GL { get; private set; } = null;

        public Point ViewBounds { get; private set; }

        private void InitEvents()
        {
            m_view.Load += OnViewLoad;
            m_view.Resize += OnViewResized;
            m_view.Closing += OnViewClosing;
        }

        private void CleanupEvents()
        {
            m_view.DoEvents();
            m_view.Reset();

            m_view.Closing -= OnViewClosing;
            m_view.Resize -= OnViewResized;
            m_view.Load -= OnViewLoad;

            m_gameLifetime.RemoveTick(this);
        }

        private IWindow InitWindowedMode(IHostEnvironment hostEnvironment)
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(1920, 1080);

            options.Title = hostEnvironment.ApplicationName;

            options.VSync = false;
            options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(4, 1));

            return Window.Create(options);
        }

        private void OnViewLoad()
        {
            // Initialize OpenGL now.
            GL = GL.GetApi(m_view);

            // Create a default 1x1 white texture as not all OpenGL implementations will do this for us.
            m_whiteTexture = new TextureObject(this, TokPixelFormat.FormatR8G8B8A8, new Point(1, 1));

            Array.Fill<byte>(m_whiteTexture.Bitmap.Data, 255);
            m_whiteTexture.Refresh();
        }

        private void OnViewResized(Vector2D<int> bounds)
        {
            ViewBounds = new Point(bounds.X, bounds.Y);
            GL.Viewport(0, 0, (uint)bounds.X, (uint)bounds.Y);
        }

        private void OnViewClosing()
        {
            m_gameLifetime.Shutdown();
        }

        public void Tick()
        {
            m_view.DoEvents();

            if (!m_view.IsClosing)
                m_view.DoUpdate();

            if (!m_view.IsClosing)
                m_view.DoRender();
        }
    }
}
