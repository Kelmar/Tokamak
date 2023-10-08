using System.Numerics;

namespace Tokamak
{
    public interface IShader : IDeviceResource
    {
        void Set(string name, int value);

        void Set(string name, float value);

        void Set(string name, in Vector2 value);

        void Set(string name, in Vector3 value);

        void Set(string name, in Vector4 value);

        void Set(string name, in Matrix3x2 value);

        void Set(string name, in Matrix4x4 value);

        dynamic this[string name]
        {
            set { Set(name, value); }
        }
    }
}
