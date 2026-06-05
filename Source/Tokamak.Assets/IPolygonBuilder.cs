using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;

using Tokamak.Mathematics;

namespace Tokamak.Assets
{
    public interface IPolygonBuilder
    {
        /// <summary>
        /// Add vertices to the current polygon.
        /// </summary>
        /// <param name="vertices">List of vertices to add.</param>
        public IPolygonBuilder AddVertices(params IEnumerable<Vector3> vertices);

        /// <summary>
        /// Add normals to the current polygon.
        /// </summary>
        /// <param name="normals">List of normals to add.</param>
        public IPolygonBuilder AddNormals(params IEnumerable<Vector3> normals);

        /// <summary>
        /// Add UV coordinates to the current polygon.
        /// </summary>
        /// <param name="uvs">List of UV coordinates to add.</param>
        public IPolygonBuilder AddUVs(params IEnumerable<Vector2> uvs);

        /// <summary>
        /// Add colors to the current polygon.
        /// </summary>
        /// <param name="colors">List of colors to add.</param>
        public IPolygonBuilder AddColors(params IEnumerable<Color> colors);

        /// <summary>
        /// Add colors to the current polygon.
        /// </summary>
        /// <param name="colors">List of colors to add.</param>
        public IPolygonBuilder AddColors(params IEnumerable<Vector4> colors);

        /// <summary>
        /// Closes the current polygon and starts a new one.
        /// </summary>
        /// <remarks>
        /// After a polygon is closed it is added the the resulting mesh of the IMeshBuilder this
        /// polygon builder was created by.  The same polygon builder can then be used to add
        /// a new polygon to the mesh.
        /// </remarks>
        public IPolygonBuilder Close();
    }
}
