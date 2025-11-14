using System;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Tritium.Scene
{
    /// <summary>
    /// The details of a scene's currently active camera.
    /// </summary>
    public class SceneCamera
    {
        private Vector3 m_up = Vector3.UnitY;
        private Vector3 m_forward = -Vector3.UnitZ;

        /// <summary>
        /// Field of view value for the view matrix
        /// </summary>
        public float FieldOfView { get; set; } = 45;

        /// <summary>
        /// Camera's location in world space.
        /// </summary>
        public Vector3 Location { get; set; }

        /// <summary>
        /// Up vector for camera.
        /// </summary>
        public Vector3 Up
        {
            get => m_up;
            set => m_up = Vector3.Normalize(value);
        }

        /// <summary>
        /// Forward vector for camera.
        /// </summary>
        public Vector3 Forward
        {
            get => m_forward;
            set => m_forward = Vector3.Normalize(value);
        }

        /// <summary>
        /// View port size.
        /// </summary>
        /// <remarks>
        /// Used for computing the view matrix.
        /// </remarks>
        public Point ViewBounds { get; set; }

        public Matrix4x4 GetViewMatrix() =>
            Matrix4x4.CreatePerspectiveFieldOfView(float.DegreesToRadians(FieldOfView), ViewBounds.X / ViewBounds.Y, 0.1f, 100f);

        /// <summary>
        /// Gets the projection matrix for the camera a it's current world location.
        /// </summary>
        public Matrix4x4 GetProjectionMatrix() =>
            Matrix4x4.CreateLookTo(Location, Forward, Up);

        /// <summary>
        /// Gets the projection matrix for the camera at the world origin.
        /// </summary>
        /// <remarks>
        /// This is used for computing the projection for environmental objects such as the sky sphere.
        /// </remarks>
        public Matrix4x4 GetFixedProjectionMatrix() =>
            Matrix4x4.CreateLookTo(Vector3.Zero, Forward, Up);
    }

}
