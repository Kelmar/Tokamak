using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Core.Config;

namespace Tokamak.Core.Drivers
{
    [Section("Drivers")]
    internal class DriverConfig
    {
        /// <summary>
        /// Selected video driver
        /// </summary>
        public string Video { get; set; }

        /// <summary>
        /// Selected audio driver
        /// </summary>
        public string Audio { get; set; }
    }
}
