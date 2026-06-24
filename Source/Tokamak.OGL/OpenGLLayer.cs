using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

using Tokamak.Logging.Abstractions;
using Tokamak.Hosting.Abstractions;

using Tokamak.Abstractions.Silk;

using Tokamak.Utilities;

using Tokamak.Mathematics;
using Tokamak.Mathematics.Silk;

using Tokamak.Tritium.APIs;

using Tokamak.Tritium.Pipelines;
using Tokamak.Tritium.Buffers;

using Monitor = Tokamak.Tritium.APIs.Monitor;
using TPixelFormat = Tokamak.Tritium.Buffers.Formats.PixelFormat;

using SilkWindow = Silk.NET.Windowing.Window;

namespace Tokamak.OGL
{
    [LogName("OpenGL")]
    internal class OpenGLLayer : IGraphicsLayer, ITick, ISilkWindow
    {
        public event SimpleEvent<Point>? OnResize;
        public event SimpleEvent<double>? OnRender;
        public event SimpleEvent? OnLoad;

        private readonly IGameLifetime m_gameLifetime;

        private bool m_firstCall = true;
        private TextureObject? m_whiteTexture = null;

        private uint m_vba = 0;

        public OpenGLLayer(IHostEnvironment hostEnvironment, IGameLifetime gameLifetime)
        {
            m_gameLifetime = gameLifetime;

            if (SilkWindow.IsViewOnly)
            {
                Window = null;
                View = SilkWindow.GetView();
            }
            else
            {
                Window = InitWindowedMode(hostEnvironment);
                View = Window;
            }

            InitEvents();

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

                    GL.BindVertexArray(0);
                    GL.DeleteVertexArray(m_vba);

                    GL.Dispose();
                }

                CleanupEvents();

                Window?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IView View { get; }

        public IWindow? Window { get; }

        public GL? GL { get; private set; } = null;

        public Point ViewBounds { get; private set; }

        public IEnumerable<Monitor> GetMonitors()
        {
            var platform = SilkWindow.GetWindowPlatform(SilkWindow.IsViewOnly);

            if (platform == null)
                throw new Exception("Unable to get window platform.");

            var mainMonitor = platform.GetMainMonitor();

            foreach (var m in platform.GetMonitors())
            {
                // Silk doesn't return the DPI info yet, hard coded for now.

                yield return new Monitor
                {
                    Index = m.Index,
                    IsMain = m.Index == mainMonitor.Index,
                    Name = m.Name,
                    Gamma = m.Gamma,
                    DPI = new Point(192, 192),
                    RawDPI = new Vector2(192, 192),
                    WorkArea = m.Bounds.ToRect()
                };
            }
        }

        private void InitEvents()
        {
            View.Load += OnViewLoad;
            View.Resize += OnViewResized;
            View.Render += OnViewRender;
            View.Closing += OnViewClosing;
        }

        private void CleanupEvents()
        {
            View.DoEvents();
            View.Reset();

            View.Closing -= OnViewClosing;
            View.Render -= OnViewRender;
            View.Resize -= OnViewResized;
            View.Load -= OnViewLoad;

            m_gameLifetime.RemoveTick(this);
        }

        private IWindow InitWindowedMode(IHostEnvironment hostEnvironment)
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(1920, 1080);

            options.Title = hostEnvironment.ApplicationName;

            options.VSync = false;
            options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(4, 1));

            return SilkWindow.Create(options);
        }

        private void OnViewLoad()
        {
            // Initialize OpenGL now.
            GL = GL.GetApi(View);

            m_vba = GL.GenVertexArray();
            GL.BindVertexArray(m_vba);

            // Create a default 1x1 white texture as not all OpenGL implementations will do this for us.
            m_whiteTexture = new TextureObject(GL, TPixelFormat.FormatR8G8B8A8, new Point(1, 1));

            Array.Fill<byte>(m_whiteTexture.Bitmap.Data, 255);
            m_whiteTexture.Refresh();

            OnViewResized(View.FramebufferSize);

            OnLoad?.Invoke();
        }

        private void OnViewResized(Vector2D<int> bounds)
        {
            Debug.Assert(GL != null, "OnViewResized() called before GL initialization?");

            ViewBounds = new Point(bounds.X, bounds.Y);
            GL.Viewport(0, 0, (uint)bounds.X, (uint)bounds.Y);
            OnResize?.Invoke(ViewBounds);
        }

        private void OnViewClosing()
        {
            m_gameLifetime.Shutdown();
        }

        private void OnViewRender(double delta)
        {
            Debug.Assert(GL != null, "OnViewRender() called before GL initialization?");

            GL.BindVertexArray(m_vba);
            OnRender?.Invoke(delta);
        }

        public void Tick()
        {
            if (m_firstCall)
            {
                /*
                 * Don't like this way of doing this, but it allows
                 * other objects to register for events before we start
                 * firing them.
                 */

                View.Initialize();
                m_firstCall = false;
            }
            else
            {
                View.DoEvents();

                if (!View.IsClosing)
                    View.DoUpdate();

                if (!View.IsClosing)
                    View.DoRender();
            }
        }

        public void SwapBuffers()
        {
            View.SwapBuffers();
        }

        public ICommandList CreateCommandList()
        {
            Debug.Assert(GL != null, "CreateCommandList() called before OpenGL initialization?");
            Debug.Assert(m_whiteTexture != null, "CreateCommandList() called before OnViewLoad()");

            return new CommandList(GL, m_whiteTexture);
        }

        public IFactory<IPipeline> GetPipelineFactory(PipelineConfig config)
        {
            Debug.Assert(GL != null, "GetPipelineFactory() called before OpenGL initialization?");

            return new PipelineFactory(GL, config);
        }

        public IVertexBuffer<T> GetVertexBuffer<T>(BufferUsage usage)
            where T : unmanaged
        {
            Debug.Assert(GL != null, "GetVertexBuffer() called before OpenGL initialization?");

            return new VertexBuffer<T>(GL, usage);
        }

        public IElementBuffer GetElementBuffer(BufferUsage usage)
        {
            Debug.Assert(GL != null, "GetElementBuffer() called before OpenGL initialization?");

            return new ElementBuffer(GL, usage);
        }

        public ITextureObject GetTextureObject(TPixelFormat format, Point size)
        {
            Debug.Assert(GL != null, "GetTextureObject() called before OpenGL initialization?");

            return new TextureObject(GL, format, size);
        }
    }
}
