using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Buffer;

namespace Tokamak
{
    public interface ICommandBuffer : IDisposable
    {
        void ClearBuffers(GlobalBuffer buffers);

        void ClearBoundTexture();

        void DrawArrays(PrimitiveType primitive, int vertexOffset, int vertexCount);

        void DrawElements(PrimitiveType primitive, int length);
    }
}
