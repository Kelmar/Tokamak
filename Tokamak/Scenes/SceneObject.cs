using System;
using System.Numerics;

namespace Tokamak.Scenes
{
    public abstract class SceneObject : IDisposable
    {
        virtual public void Dispose()
        {
        }

        public Vector3 Location { get; set; } = Vector3.Zero;

        public Vector3 Scale { get; set; } = Vector3.One;

        public Vector3 Rotation { get; set; } = Vector3.Zero;

        public Matrix4x4 Model
        {
            get
            {
                var m = Matrix4x4.Identity;

                m *= Matrix4x4.CreateTranslation(Location);
                m *= Matrix4x4.CreateScale(Scale);
                m *= Matrix4x4.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);

                return m;
            }
        }

        public abstract void Render();
    }
}
