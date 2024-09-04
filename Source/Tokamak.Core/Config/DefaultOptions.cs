﻿using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;

using Stashbox;

namespace Tokamak.Core.Config
{
    /// <summary>
    /// Class that reads the IConfiguration for a set of configuration values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class DefaultOptions<T> : IOptions<T>
        where T : class, new()
    {
        /// <summary>
        /// Used to sanitize class names
        /// </summary>
        /// <remarks>
        /// Removes the words "Option", "Options", "Config", and "Configuration" from the end of class names.
        /// </remarks>
        private static Regex s_NameClean = new("(Option(|s)|(Config(|uration)))$", RegexOptions.Compiled);

        protected readonly static ConcurrentDictionary<Type, Func<IConfiguration, object>> s_factories = new();

        public DefaultOptions(IDependencyResolver resolver)
        {
            var options = resolver.ResolveOrDefault<IConfigOptions<T>>();

            var config = resolver.Resolve<IConfiguration>();

            ConfigType = typeof(T);
            Value = new T();

            config.Bind(GetSectionName(options), Value);
        }

        private string GetSectionName(IConfigOptions<T> options)
        {
            // Options take priority.
            string section = options?.Section;

            if (String.IsNullOrWhiteSpace(section))
            {
                // Next try for attribute if no option given.
                var attr = ConfigType.GetCustomAttribute<SectionAttribute>();
                section = attr?.Section;
            }

            if (String.IsNullOrWhiteSpace(section))
            {
                // Finally default to sanitized class name.
                section = s_NameClean.Replace(ConfigType.Name, String.Empty);
            }

            return section;
        }

        public Type ConfigType { get; }

        public T Value { get; }
    }
}