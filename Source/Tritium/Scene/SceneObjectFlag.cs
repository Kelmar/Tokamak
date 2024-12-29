using System;

namespace Tokamak.Tritium.Scene
{
    /// <summary>
    /// Flags for changing the behavior of how the object is rendered/managed by the SceneManager.
    /// </summary>
    [Flags]
    public enum SceneObjectFlag
    {
        /// <summary>
        /// Informs the scene manager that this object should not 
        /// be transformed with the camera's location.  Used for
        /// sky spheres and other environmental effects.
        /// </summary>
        Environmental = 0x0000_0001
    }
}
