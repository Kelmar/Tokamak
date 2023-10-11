using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Mathematics;

namespace Tokamak
{
    public class Monitor
    {
        /// <summary>
        /// The dots per inch of the monitor.
        /// </summary>
        public Point DPI { get; set; }

        /// <summary>
        /// The detailed DPI for the monitor.
        /// </summary>
        /// <remarks>
        /// One should use the integer based DPI for most things.
        /// </remarks>
        public Vector2 RawDPI { get; set; }

        /// <summary>
        /// Gets how the OS is scaling fonts and graphics on this screen.
        /// </summary>
        /// <remarks>
        /// Windows only?
        /// </remarks>
        public float Scaling { get; set; }

        /// <summary>
        /// The working area of the monitor.
        /// </summary>
        /// <remarks>
        /// This rect describes where the monitor is in relation to the
        /// other monitors attached on the system.
        /// 
        /// Values can be negative for positions so watch out!
        /// </remarks>
        public Rect WorkArea { get; set; }
    }
}
