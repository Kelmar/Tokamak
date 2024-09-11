using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tokamak.VFS
{
    public class VirtualPath
    {
        private readonly string m_value;

        public VirtualPath(string value)
        {
            Original = value;
            m_value = Compress(value);
        }

        public string Original { get; }

        public bool IsRooted => m_value.StartsWith('/');

        public string[] Split() => m_value.Split('/');

        public string BaseFileName => Path.GetFileNameWithoutExtension(m_value);

        public string FileName => Path.GetFileName(m_value);

        public string Extension => Path.GetExtension(m_value);

        public string? Directory => Path.GetDirectoryName(m_value);

        /// <summary>
        /// Takes any relative directory names and compresses them out of the path.
        /// </summary>
        /// <returns>
        /// A path were relative '.' and '..' directory names are removed as much as possible.
        /// </returns>
        private static string Compress(string path)
        {
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            bool isRooted = path.StartsWith('/');
            var stack = new Stack<string>();
            int itemCnt = 0;

            foreach (var item in parts)
            {
                switch (item)
                {
                case ".":
                    break; // Ignore

                case "..":

                    if (itemCnt > 0)
                    {
                        stack.Pop(); // Remove the item from the stack
                        --itemCnt;
                    }
                    else if (!isRooted)
                        stack.Push(item); // We don't count top level ..

                    break;

                default:
                    stack.Push(item);
                    ++itemCnt;
                    break;
                }
            }

            var sb = new StringBuilder();

            if (path.StartsWith('/'))
                sb.Append('/');

            var leftOvers = stack.ToList();
            leftOvers.Reverse();

            sb.Append(String.Join('/', leftOvers));

            return sb.ToString();
        }

        public VirtualPath Append(VirtualPath relative)
        {
            var sb = new StringBuilder(m_value);

            if (!m_value.EndsWith('/'))
                sb.Append('/');

            sb.Append(relative.m_value);

            return new VirtualPath(sb.ToString());
        }

        public string StripRoot(string root)
        {
            string rval = m_value;

            if (m_value.StartsWith(root))
            {
                rval = m_value.Substring(root.Length);

                if (rval.StartsWith('/'))
                    rval = rval.Substring(1);
            }

            return rval;
        }

        public override string ToString() => m_value;

        public static implicit operator string(VirtualPath p) => p.m_value;
        public static implicit operator VirtualPath(string s) => new VirtualPath(s);
    }
}
