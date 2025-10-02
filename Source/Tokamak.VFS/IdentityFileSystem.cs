using System.IO;

using Tokamak.VFS.Abstractions;

namespace Tokamak.VFS
{
    /// <summary>
    /// A virtual file system implementation.
    /// </summary>
    /// <remarks>
    /// For now this is just a thin layer over the real file system.
    /// </remarks>
    public class IdentityFileSystem : FileSystem
    {
        public IdentityFileSystem(string root)
            : base(root)
        {
        }

        public override string TypeName => "Identity";

        protected override Stream InnerOpen(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return File.Open(path, mode, access, share);
        }

        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}
