namespace Tokamak.Tritium.Buffers
{
    public enum BufferUsage
    {
        /// <summary>
        /// Buffer is not modified frequently.
        /// </summary>
        Static,

        /// <summary>
        /// Buffer is expected to change frequently.
        /// </summary>
        Dynamic,

        /// <summary>
        /// Buffers for things that are intended to only be used once.
        /// </summary>
        Volatile,

        /// <summary>
        /// Buffer that cannot be modified.
        /// </summary>
        Immutable
    }
}
