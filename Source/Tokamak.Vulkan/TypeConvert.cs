using System;

using Silk.NET.Vulkan;

using Tokamak.Tritium.Pipelines;

using TBlendFactor = Tokamak.Tritium.Pipelines.BlendFactor;
using TPrimType = Tokamak.Tritium.Geometry.PrimitiveType;
using VkBlendFactor = Silk.NET.Vulkan.BlendFactor;

namespace Tokamak.Vulkan
{
    internal static class TypeConvert
    {
        public static VkBlendFactor ToVkBlend(this TBlendFactor factor)
        {
            return factor switch
            {
                TBlendFactor.Zero => VkBlendFactor.Zero,
                TBlendFactor.One => VkBlendFactor.One,
                TBlendFactor.SourceColor => VkBlendFactor.SrcColor,
                TBlendFactor.OneMinusSourceColor => VkBlendFactor.OneMinusSrcColor,
                TBlendFactor.DestColor => VkBlendFactor.DstColor,
                TBlendFactor.OneMinusDestColor => VkBlendFactor.OneMinusDstColor,
                TBlendFactor.SourceAlpha => VkBlendFactor.SrcAlpha,
                TBlendFactor.OneMinusSourceAlpha => VkBlendFactor.OneMinusSrcAlpha,
                TBlendFactor.DestAlpha => VkBlendFactor.DstAlpha,
                TBlendFactor.OneMinusDestAlpha => VkBlendFactor.OneMinusDstAlpha,
                TBlendFactor.ConstantColor => VkBlendFactor.ConstantColor,
                TBlendFactor.OneMinusConstantColor => VkBlendFactor.OneMinusConstantColor,
                TBlendFactor.SourceAlphaSaturate => VkBlendFactor.SrcAlphaSaturate,
                TBlendFactor.Source1Color => VkBlendFactor.Src1Color,
                TBlendFactor.OneMinusSource1Color => VkBlendFactor.OneMinusSrc1Color,
                TBlendFactor.Source1Alpha => VkBlendFactor.Src1Alpha,
                TBlendFactor.OneMinusSource1Alpha => VkBlendFactor.OneMinusSrc1Alpha,
                _ => throw new Exception($"Unknown blending factor: {factor}")
            };
        }

        public static PrimitiveTopology ToVkPrimitive(this TPrimType primitive)
        {
            return primitive switch
            {
                TPrimType.PointList => PrimitiveTopology.PointList,
                TPrimType.LineList => PrimitiveTopology.LineList,
                TPrimType.LineStrip => PrimitiveTopology.LineStrip,
                TPrimType.TriangleList => PrimitiveTopology.TriangleList,
                TPrimType.TriangleStrip => PrimitiveTopology.TriangleStrip,
                TPrimType.TriangleFan => PrimitiveTopology.TriangleFan,
                _ => PrimitiveTopology.PointList
            };
        }

        public static CullModeFlags ToVkCulling(this CullMode culling)
        {
            return culling switch
            {
                CullMode.None => CullModeFlags.None,
                CullMode.Back => CullModeFlags.BackBit,
                CullMode.Front => CullModeFlags.FrontBit,
                CullMode.FrontAndBack => CullModeFlags.FrontAndBack,
                _ => throw new ArgumentException($"Unknown culling mode {culling}")
            };
        }
    }
}
