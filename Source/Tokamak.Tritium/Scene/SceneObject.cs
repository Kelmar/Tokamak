using System.Numerics;

using Tokamak.Assets;
using Tokamak.Tritium.APIs;

namespace Tokamak.Tritium.Scene
{
    public abstract class SceneObject : Asset
    {
        protected SceneObject()
        {
        }

        /// <summary>
        /// The SceneManager that is handling this object.
        /// </summary>
        public SceneManager? SceneManager { get; internal set; } = null;

        /// <summary>
        /// Location of the object in the scene.
        /// </summary>
        virtual public Vector3 Location { get; set; } = Vector3.Zero;

        /// <summary>
        /// The scale of the object within the scene.
        /// </summary>
        virtual public Vector3 Scale { get; set; } = Vector3.One;

        /// <summary>
        /// The object's rotation within the scene.
        /// </summary>
        virtual public Vector3 Rotation { get; set; } = Vector3.Zero;

        virtual public Matrix4x4 Model
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

        /// <summary>
        /// Flags for changing the behavior of how the object is rendered/managed by the SceneManager.
        /// </summary>
        public SceneObjectFlag Flags { get; set; } = 0;

        public abstract void Render(ICommandList commandList);
    }
}
