using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Core.Config
{
    internal class ConfigOptions<T> : IConfigOptions<T>
        where T : class
    {
        public string Section { get; set; } = String.Empty;
    }
}
