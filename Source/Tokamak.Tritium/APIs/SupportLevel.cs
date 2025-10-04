namespace Tokamak.Tritium.APIs
{
    /// <summary>
    /// Support level for a graphics API
    /// </summary>
    /// <remarks>
    /// This enum represents a "weighted" value.
    /// The higher the value the more preferred that API is to be used.
    /// 
    /// Note that we leave a fairly large gap between values to allow for multiple APIs
    /// at the same level but still providing more preferred weights over one or the other.
    /// 
    /// For example on Mac OS X APIs might have the following weights:
    /// DirectX = 100 -- DirectX emulated through Metal
    /// OpenGL  = 200 -- Implemented, but less desirable than more optimized Vulkan layer.
    /// Vulkan  = 210 -- Prefer Vulkan over OpenGL
    /// Metal   = 300 -- Preferred native implementation.
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
