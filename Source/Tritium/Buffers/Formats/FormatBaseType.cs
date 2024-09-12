namespace Tokamak.Tritium.Buffers.Formats
{
    /// <summary>
    /// The primitive type that is used to compose a more complex format type.
    /// </summary>
    /// <remarks>
    /// E.g.: double for a 3D vector
    /// </remarks>
    public enum FormatBaseType
    {
        Byte,
        UnsignedByte,
        Short,
        UnsignedShort,
        Int,
        UnsignedInt,
        Float,
        Double
    }
}
