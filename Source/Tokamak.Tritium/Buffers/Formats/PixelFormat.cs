namespace Tokamak.Tritium.Buffers.Formats
{
    /// <summary>
    /// Format of raw pixel data.
    /// </summary>
    public enum PixelFormat
    {
        // 8-Bit Textures
        FormatA8,

        // 16-bit Textures
        FormatR5G6B5,
        FormatR5G5B5A1,

        // 24-bit formats
        FormatR8G8B8,

        // 32-bit formats
        FormatR8G8B8A8
    }
}
