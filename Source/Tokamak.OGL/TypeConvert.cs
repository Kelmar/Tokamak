using System;

using Silk.NET.OpenGL;

using Tokamak.Tritium.Buffers;

using GLShaderType = Silk.NET.OpenGL.ShaderType;
using GLPrimType = Silk.NET.OpenGL.PrimitiveType;
using GLBlendFact = Silk.NET.OpenGL.BlendingFactor;

using TPrimType = Tokamak.Tritium.Pipelines.PrimitiveType;
using FormatType = Tokamak.Tritium.Buffers.Formats.FormatBaseType;
using TPixelFormat = Tokamak.Tritium.Buffers.Formats.PixelFormat;
using TShaderType = Tokamak.Tritium.Pipelines.Shaders.ShaderType;
using TBlendFactor = Tokamak.Tritium.Pipelines.BlendFactor;

namespace Tokamak.OGL
{
    internal static class TypeConvert
    {
        public static GLBlendFact ToGLBlendFact(this TBlendFactor factor)
        {
            return factor switch
            {
                TBlendFactor.Zero => GLBlendFact.Zero,
                TBlendFactor.One => GLBlendFact.One,
                TBlendFactor.SourceColor => GLBlendFact.SrcColor,
                TBlendFactor.OneMinusSourceColor => GLBlendFact.OneMinusSrcColor,
                TBlendFactor.DestColor => GLBlendFact.DstColor,
                TBlendFactor.OneMinusDestColor => GLBlendFact.OneMinusDstColor,
                TBlendFactor.SourceAlpha => GLBlendFact.SrcAlpha,
                TBlendFactor.OneMinusSourceAlpha => GLBlendFact.OneMinusSrcAlpha,
                TBlendFactor.DestAlpha => GLBlendFact.DstAlpha,
                TBlendFactor.OneMinusDestAlpha => GLBlendFact.OneMinusDstAlpha,
                TBlendFactor.ConstantColor => GLBlendFact.ConstantColor,
                TBlendFactor.OneMinusConstantColor => GLBlendFact.OneMinusConstantColor,
                TBlendFactor.SourceAlphaSaturate => GLBlendFact.SrcAlphaSaturate,
                TBlendFactor.Source1Color => GLBlendFact.Src1Color,
                TBlendFactor.OneMinusSource1Color => GLBlendFact.OneMinusSrc1Color,
                TBlendFactor.Source1Alpha => GLBlendFact.Src1Alpha,
                TBlendFactor.OneMinusSource1Alpha => GLBlendFact.OneMinusSrc1Alpha,
                _ => throw new Exception($"Unknown blending factor: {factor}")
            };
        }

        public static GLShaderType ToGLShaderType(this TShaderType type)
        {
            return type switch
            {
                TShaderType.Fragment => GLShaderType.FragmentShader,
                TShaderType.Vertex => GLShaderType.VertexShader,
                TShaderType.Geometry => GLShaderType.GeometryShader,
                TShaderType.Compute => GLShaderType.ComputeShader,
                _ => throw new Exception($"Unknown shader type: {type}")
            };
        }

        public static GLPrimType ToGLPrimitive(this TPrimType type)
        {
            return type switch
            { 
                TPrimType.PointList => GLPrimType.Points,
                TPrimType.LineList => GLPrimType.Lines,
                TPrimType.LineStrip => GLPrimType.LineStrip,
                TPrimType.TriangleList => GLPrimType.Triangles,
                TPrimType.TriangleStrip => GLPrimType.TriangleStrip,
                _ => GLPrimType.Points
            };
        }

        public static VertexAttribPointerType ToGLType(this FormatType type)
        {
            return type switch
            {
                FormatType.Byte => VertexAttribPointerType.Byte,
                FormatType.UnsignedByte => VertexAttribPointerType.UnsignedByte,
                FormatType.Short => VertexAttribPointerType.Short,
                FormatType.UnsignedShort => VertexAttribPointerType.UnsignedShort,
                FormatType.Int => VertexAttribPointerType.Int,
                FormatType.UnsignedInt => VertexAttribPointerType.UnsignedInt,
                FormatType.Float => VertexAttribPointerType.Float,
                FormatType.Double => VertexAttribPointerType.Double,
                _ => throw new Exception($"Unknown format base type {type}")
            };
        }

        public static BufferUsageARB ToGLUsage(this BufferUsage usage)
        {
            return usage switch
            {
                BufferUsage.Static => BufferUsageARB.StaticDraw,
                BufferUsage.Dynamic => BufferUsageARB.DynamicDraw,
                BufferUsage.Volatile => BufferUsageARB.StaticDraw,
                BufferUsage.Immutable => BufferUsageARB.StaticDraw,
                _ => throw new Exception($"Unknown buffer type {usage}")
            };
        }

        public static PixelFormat ToGlPixelFormat(this TPixelFormat type)
        {
            return type switch
            {
                TPixelFormat.FormatA8 => PixelFormat.Red,
                TPixelFormat.FormatR5G6B5 => PixelFormat.Rgb,
                TPixelFormat.FormatR5G5B5A1 => PixelFormat.Rgba,
                TPixelFormat.FormatR8G8B8 => PixelFormat.Rgb,
                TPixelFormat.FormatR8G8B8A8 => PixelFormat.Rgba,
                _ => throw new Exception($"Unknown PixelFormat: {type}")
            };
        }

        public static PixelType ToGlPixelType(this TPixelFormat type)
        {
            return type switch
            {
                TPixelFormat.FormatA8 => PixelType.UnsignedByte,
                TPixelFormat.FormatR5G6B5 => PixelType.UnsignedShort565,
                TPixelFormat.FormatR5G5B5A1 => PixelType.UnsignedShort5551,
                TPixelFormat.FormatR8G8B8 => PixelType.UnsignedByte,
                TPixelFormat.FormatR8G8B8A8 => PixelType.UnsignedByte,
                _ => throw new Exception($"Unknown PixelFormat: {type}")
            };
        }

        public static InternalFormat ToGlInternalFormat(this TPixelFormat type)
        {
            // Apparently there is no R5G6B5 format here.....

            return type switch
            {
                TPixelFormat.FormatA8 => InternalFormat.R8,
                TPixelFormat.FormatR5G6B5 => InternalFormat.Rgb,
                TPixelFormat.FormatR5G5B5A1 => InternalFormat.Rgb5A1,
                TPixelFormat.FormatR8G8B8 => InternalFormat.Rgb8,
                TPixelFormat.FormatR8G8B8A8 => InternalFormat.Rgba8,
                _ => throw new Exception($"Unknown PixelFormat: {type}")
            };
        }
    }
}
