using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Tokamak.Readers.FBX
{
    internal class MaterialInfo : ResultRecord
    {
        private const string DEFAULT_SHADING_MODEL = "phong";

        [NotMapped]
        public string ShadingModel { get; set; } = DEFAULT_SHADING_MODEL;

        public Vector4 DiffuseColor { get; set; } = Vector4.One;

        public Vector4 AmbientColor { get; set; } = Vector4.One;

        public float AmbientFactor { get; set; } = 1;

        public float BumpFactor { get; set; } = 0;

        public Vector4 SpecularColor { get; set; } = Vector4.One;

        public float SpecularFactor { get; set; } = 1;

        public float Shininess { get; set; } = 0;

        public float ShininessExponent { get; set; } = 1;

        public Vector4 ReflectionColor { get; set; } = Vector4.Zero;

        public float ReflectionFactor { get; set; } = 1;
    }
}
