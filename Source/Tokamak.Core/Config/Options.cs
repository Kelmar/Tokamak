using System;
using System.Collections.Concurrent;

using Microsoft.Extensions.Configuration;

namespace Tokamak.Core.Config
{
    internal class Options<T> : IOptions<T>
        where T : class, new()
    {
        protected readonly static ConcurrentDictionary<Type, Func<IConfiguration, object>> s_factories = new();

        public Options(IConfiguration config)
        {
            ConfigType = typeof(T);
            Value = new T();
            config.Bind(typeof(T).Name, Value);
        }

        public Type ConfigType { get; }

        public T Value { get; }
    }
}
