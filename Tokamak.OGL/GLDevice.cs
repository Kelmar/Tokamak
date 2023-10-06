using System;
using System.Numerics;

namespace Tokamak.OGL
{
    public class GLDevice : Device
    {
        public override void SetViewport(int x, int y)
        {
            if (x <= 0) throw new ArgumentOutOfRangeException(nameof(x));
            if (y <= 0) throw new ArgumentOutOfRangeException(nameof(y));

            //var mat = Matrix4x4.CreateOrthographicOffCenter(0, x, y, 0, -1, 1);
            //m_projects = mat.ToOpenTK();
        }

        public override void DebugLine(in Vector2 v1, in Vector2 v2, in Color color)
        {
            
        }

        public override void DebugPoint(in Vector2 v, in Color color)
        {
            
        }

        public override void Flush()
        {
            
        }
    }
}