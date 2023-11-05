using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Tokamak.Abstractions.VFS;

namespace Tokamak.VFS
{
    internal class MountSystem : IMountSystem
    {
        private readonly List<MountInfo> m_mountPoints = new();
        private readonly IdentityFileSystem m_fileSystem = new();

        public MountSystem()
        {
            m_mountPoints.Add(new MountInfo
            {
                Root = "/",
                Type = "Identity"
            });
        }

        public IEnumerable<MountInfo> Mounts => m_mountPoints;

        public void Mount(string root, IFileSystem fileSystem)
        {
            // Empty implementation for now.
        }

        public void Unmount(string root)
        {
            // Empty implementation for now.
        }

        public Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return m_fileSystem.Open(path, mode, access, share);
        }

        public byte[] ReadAllBytes(string path)
        {
            return m_fileSystem.ReadAllBytes(path);
        }

        public string ReadAllText(string path, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8; // Default file encoding is set here as UTF8

            return encoding.GetString(ReadAllBytes(path));
        }

        public string[] ReadAllLines(string path, Encoding? encoding = null)
        {
            string text = ReadAllText(path, encoding);
            return text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        }
    }
}
