using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Readers.FBX
{
    internal class Polygon
    {
        public List<uint> Indices { get; } = new List<uint>();

        public void SplitIntoTriangles()
        {
        }
    }
}
