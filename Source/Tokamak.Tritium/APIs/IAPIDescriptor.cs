using Tokamak.Utilities;

namespace Tokamak.Tritium.APIs
{
    /// <summary>
    /// Informational description of a graphics API.
    /// </summary>
    /// <remarks>
    /// API descriptors are also factories for the API layers themselves.
    /// </remarks>
    public interface IAPIDescriptor : IFactory<IGraphicsLayer>
    {
        /// <summary>
        /// Unique ID of the graphics API
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Human readable label
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Level of support for this API on the current platform.
        /// </summary>
        SupportLevel SupportLevel { get; }
    }
}
