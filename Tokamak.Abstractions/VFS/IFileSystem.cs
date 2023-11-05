using System.IO;

namespace Tokamak.Abstractions.VFS
{
    public interface IFileSystem
    {
        Stream Open(string path, FileMode mode, FileAccess access, FileShare share);
    }
}