using System;

namespace Tokamak.Tritium.APIs
{
    /// <summary>
    /// Describes a rendering device of the platform.
    /// </summary>
    public record class DeviceInfo
    {
        /// <summary>
        /// The device ID as recognized by the platform.
        /// </summary>
        /// <remarks>
        /// For PCs this is usually the PCI device ID
        /// </remarks>
        public int DeviceID { get; init; }

        /// <summary>
        /// The vendor ID of the device as recognized by the platform.
        /// </summary>
        /// <remarks>
        /// For PCs this is usually the PCI vendor ID
        /// </remarks>
        public int VendorID { get; init; }

        /// <summary>
        /// Human readable string that names the device.
        /// </summary>
        public string Name { get; init; } = String.Empty;
    }
}
