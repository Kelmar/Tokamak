using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Abstractions.VFS;

namespace Tokamak.VFS
{
    public abstract class FileSystem : IFileSystem
    {
        protected FileSystem(string root)
        {
            Root = root;
        }

        public string Root { get; }

        public abstract string TypeName { get; }

        public Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            var vp = new VirtualPath(path);
            string relPath = vp.StripRoot(Root);
            return InnerOpen(relPath, mode, access, share);
        }

        protected abstract Stream InnerOpen(string relativePath, FileMode mode, FileAccess access, FileShare share);
    }
}
