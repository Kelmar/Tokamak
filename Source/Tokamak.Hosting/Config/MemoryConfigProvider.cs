using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Tokamak.Config.Abstractions;

namespace Tokamak.Hosting.Config
{
    public class MemoryConfigProvider : IConfigProvider
    {
        private readonly IEnumerable<KeyValuePair<string, string>> m_values;

        public MemoryConfigProvider(IEnumerable<KeyValuePair<string, string>> values)
        {
            m_values = values ?? new Dictionary<string, string>();
        }

        public IEnumerable<KeyValuePair<string, string>> GetValues()
        {
            return m_values;
        }
    }

    public class MemoryConfigProvider<T> : IConfigProvider
    {
        private readonly T m_value;

        public MemoryConfigProvider(T value)
        {
            ArgumentNullException.ThrowIfNull(value);

            m_value = value;
        }

        public IEnumerable<KeyValuePair<string, string>> GetValues()
        {
            var obj = new JObject(m_value!);
            return ConfigBuilder.RecombineJObject(obj) ?? new Dictionary<string, string>();
        }
    }
}
