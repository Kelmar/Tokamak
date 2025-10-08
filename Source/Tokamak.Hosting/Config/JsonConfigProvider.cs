using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json.Linq;

using Tokamak.Config.Abstractions;

namespace Tokamak.Hosting.Config
{
    public class JsonConfigProvider : IConfigProvider
    {
        private readonly string m_filename;
        private readonly bool m_optional;

        public JsonConfigProvider(string filename, bool optional = false)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(filename, nameof(filename));

            m_filename = filename;
            m_optional = optional;
        }

        public IEnumerable<KeyValuePair<string, string>> GetValues()
        {
            if (m_optional && !File.Exists(m_filename))
                return new Dictionary<string, string>(); // Return empty object.

            string text = File.ReadAllText(m_filename); // Will throw file not found & not optional.
            var obj = JObject.Parse(text);
            return ConfigBuilder.RecombineJObject(obj);
        }
    }
}
