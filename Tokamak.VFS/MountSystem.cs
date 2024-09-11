using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Tokamak.Abstractions.VFS;

namespace Tokamak.VFS
{
    internal class MountSystem : IMountSystem
    {
        private readonly IDictionary<string, IFileSystem> m_fileSystems = new Dictionary<string, IFileSystem>();

        public MountSystem()
        {


            m_fileSystems["/"] = new IdentityFileSystem("/");
            m_fileSystems["/sys"] = new SystemFileSystem("/sys");
        }

        public IEnumerable<MountInfo> Mounts
        {
            get
            {
                foreach (var kvp in m_fileSystems)
                {
                    yield return new MountInfo
                    {
                        Root = kvp.Key,
                        Type = kvp.Value.TypeName
                    };
                }
            }
        }

        public void Mount(string root, IFileSystem fileSystem)
        {
            string abs = Path.GetFullPath(root);

            if (abs == "/")
                throw new Exception("Cannot mount on root!");

            if (m_fileSystems.ContainsKey(abs))
                throw new Exception($"File system already mounted on {abs}");

            m_fileSystems[abs] = fileSystem;
        }

        public void Unmount(string root)
        {
            string abs = Path.GetFullPath(root);

            if (abs == "/")
                throw new Exception("Cannot unmount root!");

            m_fileSystems.Remove(abs);
        }

        private IFileSystem FindPathFS(string absolutePath)
        {
            IFileSystem rval = m_fileSystems["/"]; // Start with root

            int lastMax = 1;

            foreach (var kvp in m_fileSystems)
            {
                if (absolutePath.StartsWith(kvp.Key) && kvp.Key.Length > lastMax)
                    rval = kvp.Value;
            }

            return rval;
        }

        public Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            VirtualPath abs = path;
            var fs = FindPathFS(abs);

            return fs.Open(path, mode, access, share);
        }

        public byte[] ReadAllBytes(string path)
        {
            using var stream = Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
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
