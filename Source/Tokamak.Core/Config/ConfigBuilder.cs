using System;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json.Linq;

using Tokamak.Core.Utilities;

namespace Tokamak.Core.Config
{
    public class ConfigBuilder : IConfigBuilder
    {
        private List<IConfigProvider> m_providers = new();

        public void AddProvider(IConfigProvider provider)
        {
            m_providers.Add(provider);
        }

        public IConfiguration Build()
        {
            var rval = new Dictionary<string, string>();

            /*
             * Apply each provider in order so that later ones will
             * replace values from earlier ones.
             */
            foreach (var provider in m_providers)
            {
                var items = provider.GetValues();
                rval.Apply(items);
            }

            return new Configuration(rval);
        }

        /// <summary>
        /// Internal utility method for flattening a JObject into a list of KeyValuePairs
        /// </summary>
        /// <param name="o">The JObject to flatten</param>
        /// <param name="basePath">The base path of the given JObject</param>
        /// <returns>A list of KeyValuePairs for the given object.</returns>
        internal static IEnumerable<KeyValuePair<string, string>> RecombineJObject(JObject o, string basePath = "")
        {
            Debug.Assert(o != null);

            var rval = new List<KeyValuePair<string, string>>();
            basePath ??= String.Empty;

            foreach (var child in o.Children())
            {
                var p = child as JProperty;

                string key = p!.Name;

                string fullPath = ConfigPath.Combine(basePath, key);

                if (String.IsNullOrWhiteSpace(fullPath))
                    continue; // Cannot make a valid entry, skip

                switch (p.Value.Type)
                {
                case JTokenType.Object:
                case JTokenType.Array:
                    rval.AddRange(RecombineJObject((JObject)p.Value, fullPath));
                    break;

                case JTokenType.Null:
                case JTokenType.Undefined:
                case JTokenType.None:
                case JTokenType.Comment:
                case JTokenType.Constructor:
                    continue;

                default:
                    rval.Add(new(fullPath, p.Value.ToString()));
                    break;
                }
            }

            return rval;
        }
    }
}
