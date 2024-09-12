using System.Numerics;
using Tokamak.Mathematics;

namespace Tokamak.Tritium.APIs
{
    /// <summary>
    /// Structures for holding details about a connected host display.
    /// </summary>
    public record class Monitor
    {
        /// <summary>
        /// The index of the monitor.
        /// </summary>
        public int Index { get; init; }

        /// <summary>
        /// Gets the flag indicating if this is the main monitor or not.
        /// </summary>
        public bool IsMain { get; init; }

        /// <summary>
        /// Name of the monitor registered with the OS if any.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets the gamma setting for this monitor.
        /// </summary>
        public float Gamma { get; init; }

        /// <summary>
        /// The dots per inch of the monitor.
        /// </summary>
        public Point DPI { get; init; }

        /// <summary>
        /// The detailed DPI for the monitor.
        /// </summary>
        /// <remarks>
        /// One should use the integer based DPI for most things.
        /// </remarks>
        public Vector2 RawDPI { get; init; }

        /// <summary>
        /// Gets how the OS is scaling fonts and graphics on this screen.
        /// </summary>
        /// <remarks>
        /// Windows only?
        /// </remarks>
        public float Scaling { get; init; }

        /// <summary>
        /// The working area of the monitor.
        /// </summary>
        /// <remarks>
        /// This rect describes where the monitor is in relation to the
        /// other monitors attached on the system.
        /// 
        /// Values can be negative for positions so watch out!
        /// </remarks>
        public Rect WorkArea { get; init; }
    }
}
