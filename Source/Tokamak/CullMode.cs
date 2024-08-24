using System;

namespace Tokamak
{
    [Flags]
    public enum CullMode
    {
        /// <summary>
        /// Disable culling
        /// </summary>
        None            = 0,

        /// <summary>
        /// Cull back faces
        /// </summary>
        Back            = 1,

        /// <summary>
        /// Cull front faces
        /// </summary>
        Front           = 2,

        /// <summary>
        /// Cull both back and front faces?
        /// </summary>
        FrontAndBack    = 3
    }
}
