using System.IO;

namespace Tokamak.VFS.Abstractions
{
    public interface IFileSystem
    {
        string TypeName { get; }

        Stream Open(string path, FileMode mode, FileAccess access, FileShare share);
    }
}
