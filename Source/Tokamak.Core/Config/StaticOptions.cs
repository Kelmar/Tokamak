﻿using Tokamak.Abstractions.Config;

namespace Tokamak.Core.Config
{
    /// <summary>
    /// Simple class for providing static configuration for an item.
    /// </summary>
    /// <typeparam name="T">The configuration class</typeparam>
    internal class StaticOptions<T> : IOptions<T>
        where T : class
    {
        public StaticOptions(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}
