namespace FreeTypeWrapper
{
    public struct BitmapSize
    {
        /// <summary>
        /// The vertical distance, in pixels, between two consecutive baselines.  It is always positive.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The average width, in pixels, of all glyphs in the strike.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The nominal size of the strike in 26.6 factional points.
        /// </summary>
        /// <remarks>
        /// This field is not very useful.
        /// </remarks>
        public int Size { get; set; }

        /// <summary>
        /// The horizontal ppem (nominal width) in 26.6 fractional pixels.
        /// </summary>
        public int XPpem { get; set; }

        /// <summary>
        /// The vertical ppem (nominal height) in 26.6 fractional pixels.
        /// </summary>
        public int YPpem { get; set; }

    }
}
