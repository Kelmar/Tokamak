using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using Tokamak.Abstractions.Config;

namespace Tokamak.Core.Config
{
    public static class ConfigExtensions
    {
        public static void ReadInto<T>(this IConfigSection section, T value)
        {
            // This is a slow way of doing this, but it works.

            Debug.Assert(value != null);

            var t = value.GetType();
            var props = t
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var p in props)
            {
                bool ignore =
                    p.GetCustomAttribute<IgnoreDataMemberAttribute>() != null ||
                    p.GetCustomAttribute<NotMappedAttribute>() != null;

                if (ignore)
                    continue;

                string key = p.GetSectionKey();

                if (p.PropertyType.IsPrimitive)
                {
                    // Read a simple property
                    string propValStr = section[key];

                    if (!String.IsNullOrWhiteSpace(propValStr))
                        p.SetValue(value, Convert.ChangeType(propValStr, p.PropertyType));
                }
                else if (p.PropertyType == typeof(string))
                {
                    string propValStr = section[key];

                    if (!String.IsNullOrWhiteSpace(propValStr))
                        p.SetValue(value, propValStr);
                }
                else
                {
                    // Read subsection of the config
                    var subSection = section.GetSection(key);
                    var val = Activator.CreateInstance(p.PropertyType);
                    subSection.ReadInto(val);
                    p.SetValue(value, val);
                }
            }
        }

        /// <summary>
        /// Deduce the section name to use for a given property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static string GetSectionKey(this PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<SectionAttribute>();

            if (attr == null && property.PropertyType.IsClass)
                attr = property.PropertyType.GetCustomAttribute<SectionAttribute>();

            return attr?.Section ?? property.Name;
        }

        public static IConfigBuilder AddJsonFile(this IConfigBuilder builder, string filename, bool optional = false)
        {
            builder.AddProvider(new JsonConfigProvider(filename, optional));
            return builder;
        }

        public static IConfigBuilder AddInMemoryConfig(this IConfigBuilder builder, IEnumerable<KeyValuePair<string, string>>? values = null)
        {
            builder.AddProvider(new MemoryConfigProvider(values ?? []));
            return builder;
        }

        public static IConfigBuilder AddConfiguration(this IConfigBuilder builder, IConfiguration config)
        {
            builder.AddProvider(new MemoryConfigProvider(config));
            return builder;
        }

        /// <summary>
        /// Add environment variables that match the given pattern to the configuration list.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="pattern"></param>
        public static IConfigBuilder AddEnvironmentVariables(
            this IConfigBuilder builder,
            string pathRoot,
            [StringSyntax("Regex")] string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var item in Environment.GetEnvironmentVariables())
            {
                var entry = (DictionaryEntry)item;
                string? key = entry.Key.ToString();
                string? value = entry.Value?.ToString();

                if (String.IsNullOrWhiteSpace(key))
                    continue;
                
                var matches = regex.Matches(key);

                if (matches.Count > 0 && !String.IsNullOrWhiteSpace(value))
                {
                    string configKey = ConfigPath.Combine(pathRoot, matches[0].Groups[1].Value);
                    result[configKey] = value;
                }
            }

            if (result.Any())
                builder.AddInMemoryConfig(result);

            return builder;
        }

        /// <summary>
        /// Parse the supplied command line arguments into the builder's list of providers.
        /// </summary>
        /// <param name="builder">The builder to add the arguments to.</param>
        /// <param name="args">The list of arguments to add.</param>
        public static IConfigBuilder ParseCommandArgs(this IConfigBuilder builder, string[] args)
        {
            return builder;
        }
    }
}
