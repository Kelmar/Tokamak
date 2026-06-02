using System.Collections.Generic;
using System.Numerics;

namespace Tokamak.Readers.FBX.DOM
{
    /// <summary>
    /// A single polygon inside of a FBXMesh.
    /// </summary>
    /// <remarks>
    /// The reader makes sure that all lists are of the same length.
    /// 
    /// That is to say that after reading everything is normalized to be by vertex.
    /// </remarks>
    internal class FBXPolygon
    {
        public List<Vector3> Vectors { get; } = [];

        public List<Vector3> Normals { get; } = [];

        public List<Vector2> TexCoord { get; } = [];

        /// <summary>
        /// Materials might not be loaded yet, so we just store the index
        /// until we're ready to convert to a proper reference.
        /// </summary>
        public List<int> Material { get; } = [];
    }
}
