using System;

namespace Tokamak.Tritium.APIs
{
    /// <summary>
    /// Informational description of a graphics API.
    /// </summary>
    public interface IAPIDescriptor
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

        /// <summary>
        /// Factory method for creating API management object.
        /// </summary>
        /// <returns></returns>
        IDisposable Create();
    }
}
