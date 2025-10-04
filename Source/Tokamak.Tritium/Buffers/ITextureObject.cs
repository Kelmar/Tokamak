using Tokamak.Mathematics;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Buffers.Formats;

namespace Tokamak.Tritium.Buffers
{
    public interface ITextureObject : IDeviceResource
    {
        /// <summary>
        /// Pixel format of the texture.
        /// </summary>
        PixelFormat Format { get; }

        /// <summary>
        /// Size of the texture in pixels.
        /// </summary>
        /// <remarks>
        /// Note that textures must be powers of two.
        /// </remarks>
        Point Size { get; }

        /// <summary>
        /// Gets the bitmap backing store for this texture.
        /// </summary>
        Bitmap Bitmap { get; }

        /// <summary>
        /// Refresh the GPU copy of the texture.
        /// </summary>
        void Refresh();
    }
}
