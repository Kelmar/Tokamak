using System;
using System.Collections.Generic;
using System.IO;

namespace Tokamak.Config
{
    /// <summary>
    /// This is the most basic of brain dead config systems at the moment.
    /// </summary>
    /// <remarks>
    /// Just parses a simple text file as a set of assignments.
    /// 
    /// TODO: Replace this with a more comprehensive configuration system 
    /// that can do change tracking and nofitications.
    /// </remarks>
    public class BasicConfigReader : IConfigReader
    {
        private IDictionary<string, string> m_values;

        public BasicConfigReader()
        {
            var lines = File.ReadAllLines("config.txt");
            m_values = ParseLines(lines);
        }

        private static IDictionary<string, string> ParseLines(string[] lines)
        {
            var rval = new Dictionary<string, string>();

            foreach (var line in lines)
            {
                string l = line.Trim();

                if (l.StartsWith("#"))
                    continue; // Ignore comments

                var idx = l.IndexOf('=');

                if (idx == -1)
                    rval[l] = "true"; // Just mark the line as present
                else
                {
                    var name = l.Substring(0, idx - 1).Trim();
                    rval[name] = l.Substring(idx + 1).Trim();
                }    
            }

            return rval;
        }

        public T Get<T>(string name, T defValue = default)
        {
            if (m_values.TryGetValue(name, out string val))
                return (T)Convert.ChangeType(val, typeof(T));

            return defValue;
        }
    }
}
