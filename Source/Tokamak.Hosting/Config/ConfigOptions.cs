using System;

using Tokamak.Config.Abstractions;

namespace Tokamak.Hosting.Config
{
    internal class ConfigOptions<T> : IConfigOptions<T>
        where T : class
    {
        public string Section { get; set; } = String.Empty;
    }
}
