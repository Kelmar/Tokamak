using System;
using System.Collections.Generic;
using System.IO;

namespace Tokamak.VFS
{
    /// <summary>
    /// This provides an abstraction file system for system resources.
    /// </summary>
    /// <remarks>
    /// It's primary purpose is to map from /sys/resourceType/resourceName to where
    /// the actual file lives on a given platform.
    /// 
    /// E.g.: /sys/font/arial.ttf
    /// On Windows would map to: C:\Windows\Fonts\arial.ttf
    /// </remarks>
    internal class SystemFileSystem : FileSystem
    {
        private readonly IDictionary<string, string> m_systemPaths = new Dictionary<string, string>();

        public SystemFileSystem(string root)
            : base(root)
        {
            InitSystemPaths();
        }

        override public string TypeName => "System";

        private void InitSystemPaths()
        {
            // For now we just have the one for Windows
            m_systemPaths["fonts"] = Path.Combine(Environment.SystemDirectory, "../Fonts");
        }

        override protected Stream InnerOpen(string path, FileMode mode, FileAccess access, FileShare share)
        {
            string[] parts = path.Split('/', 2);

            if (parts.Length == 0)
                throw new FileNotFoundException();

            // The first chunk of the path is the type we're looking for.
            // The second chunk is the file name that is relative to the system's root for that resource type.

            if (!m_systemPaths.ContainsKey(parts[0]))
                throw new FileNotFoundException();

            string realPath = m_systemPaths[parts[0]];
            string fullPath = Path.Combine(realPath, parts[1]);

            return File.Open(fullPath, mode, access, share);
        }
    }
}
