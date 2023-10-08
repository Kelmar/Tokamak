using System;

using OpenTK.Graphics.OpenGL4;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using Tokamak;
using Tokamak.Mathematics;

using Graphite;

namespace TestBed
{
    public class MainWindow : GameWindow
    {
        private static readonly Color TransWhite = new Color(255, 255, 255, 128);

        private static readonly Color TransRed = new Color(255, 0, 0, 128);

        private readonly Device m_device;
        private readonly Canvas m_canvas;

        private decimal m_miter = 3;
        private bool m_trans = false;

        public MainWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            m_device = new Tokamak.OGL.GLDevice();
            m_canvas = new Canvas(m_device);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_canvas.Dispose();
                m_device.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            /*
            if (KeyboardState.IsKeyReleased(Keys.W))
                m_render.WireFrame = !m_render.WireFrame;

            if (KeyboardState.IsKeyReleased(Keys.D))
                m_render.Debug = !m_render.Debug;
            */

            if (KeyboardState.IsKeyReleased(Keys.T))
                m_trans = !m_trans;

            decimal oldMiter = m_miter;

            if (KeyboardState.IsKeyReleased(Keys.Up))
                m_miter = Math.Min(m_miter + 0.1m, 3);

            if (KeyboardState.IsKeyReleased(Keys.Down))
                m_miter = Math.Max(m_miter - 0.1m, 0.1m);

            if (m_miter != oldMiter)
            {
                Title = String.Format("GL Test : Miter:{0}", m_miter);
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Pen pen = new Pen
            {
                Width = 40,
                Color = m_trans ? TransWhite : Color.White,
                MiterLimit = (float)m_miter,
                //LineJoin = LineJoin.Bevel
            };

            Rect r = new Rect(200, 200, 100, 100);
            m_canvas.StrokeRect(pen, r);

            //var p1 = new Path();

            //p1.MoveTo(50, 50);
            //p1.LineTo(500, 50);
            //p1.LineTo(500, 500);
            //p1.Closed = true;

            //m_canvas.StrokePath(pen, p1);

            ////var p2 = new Path();
            ////p2.Rect(200, 200, 100, 100);

            ////m_context.StrokePath(pen, p2);

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

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            if (e.Width > 0 && e.Height > 0)
                m_device.Viewport = new Rect(0, 0, e.Width, e.Height);
        }
    }
}
