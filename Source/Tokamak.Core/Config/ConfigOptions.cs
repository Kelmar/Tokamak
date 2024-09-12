using System;

namespace Tokamak.Core.Config
{
    internal class ConfigOptions<T> : IConfigOptions<T>
        where T : class
    {
        public string Section { get; set; } = String.Empty;
    }
}
