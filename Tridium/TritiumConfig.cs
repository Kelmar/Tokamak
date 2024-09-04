using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Tritium
{
    public class TritiumConfig
    {
        /// <summary>
        /// Gets or sets which graphics API to use.
        /// </summary>
        /// <example>OpenGL</example>
        public string API { get; set; }

        /// <summary>
        /// Set if the application is running "headless" mode.
        /// (No UI/Renderer)
        /// </summary>
        public bool Headless { get; set; }
    }
}
