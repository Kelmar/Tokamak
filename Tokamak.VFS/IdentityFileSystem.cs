using System.IO;

using Tokamak.Abstractions.VFS;

namespace Tokamak.VFS
{
    /// <summary>
    /// A virtual file system implementation.
    /// </summary>
    /// <remarks>
    /// For now this is just a thin layer over the real file system.
    /// </remarks>
    public class IdentityFileSystem : IFileSystem
    {
        public Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return File.Open(path, mode, access, share);
        }

        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}
