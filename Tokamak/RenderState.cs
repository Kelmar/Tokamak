using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak
{
    public class RenderState
    {
        public bool UseDepthTest { get; set; }

        public bool CullFaces { get; set; }

        public Color ClearColor { get; set; } = Color.Black;
    }
}
