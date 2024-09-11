using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tokamak.Abstractions.VFS
{
    public interface IMountSystem
    {
        IEnumerable<MountInfo> Mounts { get; }

        void Mount(string root, IFileSystem fileSystem);

        void Unmount(string root);

        Stream Open(string path, FileMode mode, FileAccess access, FileShare share = FileShare.None);

        byte[] ReadAllBytes(string path);

        string ReadAllText(string path, Encoding? encoding = null);

        string[] ReadAllLines(string path, Encoding? encoding = null);
    }
}
