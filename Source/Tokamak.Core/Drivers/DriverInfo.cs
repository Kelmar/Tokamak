namespace Tokamak.Core.Drivers
{
    public record DriverInfo
    {
        /// <summary>
        /// Unique ID of the driver
        /// </summary>
        /// <remarks>
        /// Used for identification in configuration.
        /// E.g. "opengl", "openal", "d3d", "vulkan", etc.
        /// </remarks>
        public string Id { get; init; }

        /// <summary>
        /// Path to the driver's assembly.
        /// </summary>
        public string Path { get; init; }

        /// <summary>
        /// Pretty human name of the driver.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The type of service the driver provides.
        /// </summary>
        public DriverType DriverType { get; init; }
    }
}
