using System.Collections.Generic;
using System.Numerics;

namespace Tokamak.Tritium.Buffers
{
    public class Polygon
    {
        public List<Vector3> Vectors { get; init; } = new();

        public List<Vector3> Normals { get; init; } = new();

        public List<Vector2> TexCoord { get; init; } = new();

        private Polygon MakeTriangle(int i1, int i2, int i3) => new Polygon
        {
            Vectors  = [Vectors[i1] , Vectors[i2] , Vectors[i3] ],
            Normals  = [Normals[i1] , Normals[i2] , Normals[i3] ],
            TexCoord = [TexCoord[i1], TexCoord[i2], TexCoord[i3]]
        };

        public IEnumerable<Polygon> SplitIntoTriangles()
        {
            if (Vectors.Count < 4)
            {
                yield return this;
                yield break;
            }

            // For now we do a simple split, making the assumption that the polygon is convex.

            int lastIdx = 1;

            for (int i = 2; i < Vectors.Count; ++i)
            {
                yield return MakeTriangle(0, lastIdx, i);
                lastIdx = i;
            }
        }
    }
}
