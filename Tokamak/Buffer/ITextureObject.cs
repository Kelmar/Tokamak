using Tokamak.Formats;
using Tokamak.Mathematics;

namespace Tokamak.Buffer
{
    public interface ITextureObject : IDeviceResource
    {
        PixelFormat Format { get; }

        Point Size { get; }

        void Set(int mipLevel, byte[] data);
    }
}
