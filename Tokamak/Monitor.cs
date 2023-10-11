using System.Numerics;

using Silk.NET.Maths;

using Tokamak.Mathematics;

namespace Tokamak
{
    public class Monitor
    {
        /// <summary>
        /// The index of the monitor.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets the flag indicating if this is the main monitor or not.
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// Gets the gamma setting for this monitor.
        /// </summary>
        public float Gamma { get; set; }

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
        public Rectangle<int> WorkArea { get; set; }
    }
}
