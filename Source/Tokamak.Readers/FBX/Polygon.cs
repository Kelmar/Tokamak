using System.Collections.Generic;

namespace Tokamak.Readers.FBX
{
    internal class Polygon
    {
        public List<uint> Indices { get; } = new List<uint>();

        public IEnumerable<Polygon> SplitIntoTriangles()
        {
            if (Indices.Count < 4)
            {
                yield return this;
                yield break;
            }

            // For now we do a simple split, making the assumption that the polygon is convex.

            uint first = Indices[0];
            uint last = Indices[1];

            for (int i = 2; i < Indices.Count; ++i)
            {
                var poly = new Polygon();

                poly.Indices.Add(first);
                poly.Indices.Add(last);
                poly.Indices.Add(Indices[i]);

                yield return poly;

                //last0 = last1;
                //last1 = Indices[i];
                last = Indices[i];
            }
        }
    }
}
