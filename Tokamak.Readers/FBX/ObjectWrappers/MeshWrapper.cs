using Tokamak.Buffer;

namespace Tokamak.Readers.FBX.ObjectWrappers
{
    /// <summary>
    /// Wrapper around a mesh object for loading FBX files.
    /// </summary>
    internal class MeshWrapper : IFBXObject
    {
        /// <summary>
        /// The unique ID of this object in the file.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The name of this mesh in the file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The FBX node we read from.
        /// </summary>
        public Node Node { get; set; }

        /// <summary>
        /// Engine mesh object that was built up from this node's data.
        /// </summary>
        public Mesh Mesh { get; set; }
    }
}
