using System.Runtime.InteropServices;

namespace Graphite.OGL
{
    /// <summary>
    /// Holds 2D coordinates as well as UV information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Vertex : IVertex
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float U { get; set; }

        public float V { get; set; }
    }
}
