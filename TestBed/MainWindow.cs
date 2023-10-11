using System;
using System.IO;

using StbImageSharp;

using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

using Tokamak;
using Tokamak.Buffer;
using Tokamak.Mathematics;

using Graphite;


namespace TestBed
{
    public class MainWindow : IDisposable
    {
        private static readonly Color TransWhite = new Color(255, 255, 255, 128);

        private static readonly Color TransRed = new Color(255, 0, 0, 128);

        private readonly IWindow m_silkWindow;

        private Tokamak.OGL.GLDevice m_device;
        private Canvas m_canvas;
        private ITextureObject m_texture;
        private Font m_font;

        private bool m_trans = false;

        private ITextureObject m_letter;

        public MainWindow()
        {
            var options = WindowOptions.Default;
            //options.ShouldSwapAutomatically = true;
            options.Size = new Vector2D<int>(1920, 1080);
            options.Title = "OpenGL Test SILK!";
            //options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(4, 1));

            m_silkWindow = Window.Create(options);

            m_silkWindow.Load += OnLoad;
            m_silkWindow.Closing += OnClosing;
            m_silkWindow.Update += OnUpdateFrame;
            m_silkWindow.Render += OnRenderFrame;
            m_silkWindow.Resize += OnResize;
        }

        private void OnClosing()
        {
            m_letter.Dispose();
            m_font.Dispose();
            m_canvas.Dispose();
            m_device.Dispose();
        }

        private void OnLoad()
        {
            m_device = new Tokamak.OGL.GLDevice(m_silkWindow);
            m_canvas = new Canvas(m_device);

            using var shaderFact = m_device.GetShaderFactory();

            m_texture = LoadTexture();

            string path = Path.Combine(Environment.SystemDirectory, "../Fonts/arial.ttf");
            m_font = m_canvas.GetFont(path, 8);

            m_letter = m_font.DrawGlyph('G');

            OnResize(m_silkWindow.Size);
        }

        public void Dispose()
        {
            m_silkWindow.Dispose();
        }

        public void Run() => m_silkWindow.Run();

        private ITextureObject LoadTexture()
        {
            StbImage.stbi_set_flip_vertically_on_load(1);

            using var s = File.OpenRead("resources/container.png");
            ImageResult image = ImageResult.FromStream(s);

            Point size = new Point(image.Width, image.Height);

            var bits = new Bitmap(size, Tokamak.Formats.PixelFormat.FormatR8G8B8);
            bits.Blit(image.Data, new Point(0, 0), image.Width, image.Width * 3);

            ITextureObject rval = m_device.GetTextureObject(bits.Format, bits.Size);
            rval.Set(bits);

            return rval;
        }

        protected void OnUpdateFrame(double delta)
        {
            //if (KeyboardState.IsKeyDown(Keys.Escape))
            //    Close();

            /*
            if (KeyboardState.IsKeyReleased(Keys.W))
                m_render.WireFrame = !m_render.WireFrame;

            if (KeyboardState.IsKeyReleased(Keys.D))
                m_render.Debug = !m_render.Debug;
            */

            //if (KeyboardState.IsKeyReleased(Keys.T))
            //    m_trans = !m_trans;
        }

        protected void OnRenderFrame(double delta)
        {
            // TODO: Abstract this GL call.
            m_device.GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Pen pen = new Pen
            {
                Width = 40,
                Color = m_trans ? TransWhite : Color.DarkRed,
                //LineJoin = LineJoin.Bevel
            };

            Rect r = new Rect(200, 200, 100, 100);
            m_canvas.StrokeRect(pen, r);

            m_canvas.DrawImage(m_texture, new Point(500, 500));

            m_canvas.DrawImage(m_letter, new Point(510, 510));

            //var p1 = new Path();

            //p1.MoveTo(50, 50);
            //p1.LineTo(500, 50);
            //p1.LineTo(500, 500);
            //p1.Closed = true;

            //m_canvas.StrokePath(pen, p1);

            //var p3 = new Path();
            ////p3.Rect(50, 50, 450, 450);
            //p3.MoveTo(550, 50);
            //p3.LineTo(950, 50);
            //p3.LineTo(950, 500);
            //p3.LineTo(1200, 750);
            //p3.LineTo(1450, 500);
            //p3.LineTo(1450, 50);

            //pen.Color = m_trans ? TransRed : Color.LiteRed;

            //m_canvas.StrokePath(pen, p3);

            m_canvas.Flush();

            //m_silkWindow.SwapBuffers();
        }

        protected void OnResize(Vector2D<int> size)
        {
            if (size.X > 0 && size.Y > 0)
            {
                m_device.Viewport = new Rect(0, 0, size.X, size.Y);
                m_canvas.SetSize(size.X, size.Y);
            }
        }
    }
}
