
using Tokamak.Mathematics;

namespace Tokamak.Tritium.Rendering
{
    public class Material
    {
        /// <summary>
        /// Diffuse color of the material.
        /// </summary>
        /// <remarks>Default: white</remarks>
        public Color DiffuseColor { get; set; } = Color.White;

        /// <summary>
        /// Ambient color of the material.
        /// </summary>
        /// <remarks>Default: white</remarks>
        public Color AmbientColor { get; set; } = Color.White;

        /// <summary>
        /// Specular color of the material.
        /// </summary>
        /// <remarks>Default: white</remarks>
        public Color SpecularColor { get; set; } = Color.White;

        /// <summary>
        /// Emissive color of the material.
        /// </summary>
        /// <remarks>Default: black (no emission)</remarks>
        public Color EmissiveColor { get; set; } = Color.Black;

        /// <summary>
        /// Bump map scaling factor
        /// </summary>
        /// <remarks>
        /// Default: 1 (full bump)<br />
        /// <br />
        /// Recommended to keep this value from 0 (off) to 1 (full)
        /// </remarks>
        public float BumpFactor { get; set; } = 1;
    }
}
