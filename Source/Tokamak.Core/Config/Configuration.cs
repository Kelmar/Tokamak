using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Tokamak.Abstractions.Config;

using Tokamak.Core.Utilities;

namespace Tokamak.Core.Config
{
    internal sealed class Configuration : IConfiguration
    {
        private readonly Dictionary<string, string>? m_items;

        internal Configuration(IEnumerable<KeyValuePair<string, string>> items)
        {
            m_items = new(items, StringComparer.InvariantCultureIgnoreCase);

            Root = this;
            Path = String.Empty;
        }

        internal Configuration(IConfiguration root, string path)
        {
            Debug.Assert(root != null);
            Debug.Assert(!String.IsNullOrWhiteSpace(path));

            m_items = null;
            Root = root;
            Path = path;
        }

        public INotifier<ConfigNotice> OnChanged { get; } = new Notifier<ConfigNotice>();

        public IConfiguration Root { get; }

        public string Path { get; }

        public IEnumerable<string> Keys
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<string> Values
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string this[string path]
        {
            get => Get(path);
            set => Set(path, value);
        }

        private void SendNotice(string key, string value)
        {
            OnChanged.Raise(new ConfigNotice
            {
                Key = key,
                Value = value
            });
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new NotImplementedException();
            //return m_items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
            //return m_items.GetEnumerator();
        }

        public string Get(string path)
        {
            if (Root == this)
            {
                Debug.Assert(m_items != null);

                if (!ConfigPath.IsValidPath(path))
                    throw new ArgumentException("Invalid config path");

                if (m_items.TryGetValue(path, out string? value))
                    return value ?? String.Empty;

                return String.Empty;
            }

            string key = ConfigPath.Combine(Path, path);
            return Root.Get(key);
        }

        public void Set(string path, string value)
        {
            if (Root == this)
            {
                Debug.Assert(m_items != null);

                if (!ConfigPath.IsValidPath(path))
                    throw new ArgumentException("Invalid config path");

                if (!m_items!.TryGetValue(path, out string? old))
                    old = String.Empty;

                m_items[path] = value;

                if (old != value)
                    SendNotice(path, value);

                return;
            }

            string key = ConfigPath.Combine(Path, path);
            Root.Set(key, value);
        }

        public IConfigSection GetSection(string path)
        {
            if (!ConfigPath.IsValidPath(path))
                throw new ArgumentNullException("Invalid config path");

            string key = ConfigPath.Combine(Path, path);

            return new Configuration(Root, key);
        }

        public void Revert()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
