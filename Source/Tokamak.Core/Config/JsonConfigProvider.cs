using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json.Linq;

namespace Tokamak.Core.Config
{
    public class JsonConfigProvider : IConfigProvider
    {
        private readonly string m_filename;
        private readonly bool m_optional;

        public JsonConfigProvider(string filename, bool optional = false)
        {
            m_filename = filename;
            m_optional = optional;
        }

        public IEnumerable<KeyValuePair<string, string>> GetValues()
        {
            if (m_optional)
            {
                if (!File.Exists(m_filename))
                    return new Dictionary<string, string>(); // Return empty object.
            }

            string text = File.ReadAllText(m_filename);
            var obj = JObject.Parse(text);
            return ConfigBuilder.RecombineJObject(obj);
        }
    }
}
