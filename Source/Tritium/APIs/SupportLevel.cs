using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Tritium.APIs
{
    /// <summary>
    /// Support level for a graphics API
    /// </summary>
    /// <remarks>
    /// This enum represents a "weighted" value.
    /// The higher the value the more preferred that API is to be used.
    /// </remarks>
    public enum SupportLevel
    {
        /// <summary>
        /// API is not supported on this platform.
        /// </summary>
        NoSupport = 0,

        /// <summary>
        /// API is supported via emulation through another API.
        /// </summary>
        Emulated = 100,

        /// <summary>
        /// API has a native implementation but isn't the preferred implementation of that platform.
        /// </summary>
        Native = 200,

        /// <summary>
        /// The platform's preferred native graphics implementation.
        /// </summary>
        Preferred = 300
    }
}
