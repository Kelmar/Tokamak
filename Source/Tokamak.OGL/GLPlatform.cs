using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Silk.NET.OpenGL;
using Silk.NET.Core.Contexts;
using Silk.NET.Windowing;

using Tokamak.Mathematics;


namespace Tokamak.OGL
{
    public class GLPlatform : IDisposable
    {
        public GLPlatform(IGLContext context)
            : base()
        {
            GL = GL.GetApi(context);

            // Need to figure out how to abstract these.
            GL.Enable(EnableCap.Blend);

            // A good blending function for 2D font antialiasing.
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
        }

        public void Dispose()
        {
            GL.Dispose();

            //base.Dispose();
        }

        public GL GL { get; }


    }
}