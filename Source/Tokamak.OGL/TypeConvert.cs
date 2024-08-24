using System;

using Silk.NET.OpenGL;

using Tokamak.Buffer;

using GLPrimType = Silk.NET.OpenGL.PrimitiveType;

using TPrimType = Tokamak.PrimitiveType;
using FormatType = Tokamak.Formats.FormatBaseType;
using TokPixelFormat = Tokamak.Formats.PixelFormat;

namespace Tokamak.OGL
{
    internal static class TypeConvert
    {
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

        public static BufferUsageARB ToGLType(this BufferType type)
        {
            return type switch
            {
                BufferType.Static => BufferUsageARB.StaticDraw,
                BufferType.Dynamic => BufferUsageARB.DynamicDraw,
                BufferType.Volatile => BufferUsageARB.StaticDraw,
                BufferType.Immutable => BufferUsageARB.StaticDraw,
                _ => throw new Exception($"Unknown buffer type {type}")
            };
        }

        public static PixelFormat ToGlPixelFormat(this TokPixelFormat type)
        {
            return type switch
            {
                TokPixelFormat.FormatA8 => PixelFormat.Red,
                TokPixelFormat.FormatR5G6B5 => PixelFormat.Rgb,
                TokPixelFormat.FormatR5G5B5A1 => PixelFormat.Rgba,
                TokPixelFormat.FormatR8G8B8 => PixelFormat.Rgb,
                TokPixelFormat.FormatR8G8B8A8 => PixelFormat.Rgba,
                _ => throw new Exception($"Unknown PixelFormat: {type}")
            };
        }

        public static PixelType ToGlPixelType(this TokPixelFormat type)
        {
            return type switch
            {
                TokPixelFormat.FormatA8 => PixelType.UnsignedByte,
                TokPixelFormat.FormatR5G6B5 => PixelType.UnsignedShort565,
                TokPixelFormat.FormatR5G5B5A1 => PixelType.UnsignedShort5551,
                TokPixelFormat.FormatR8G8B8 => PixelType.UnsignedByte,
                TokPixelFormat.FormatR8G8B8A8 => PixelType.UnsignedByte,
                _ => throw new Exception($"Unknown PixelFormat: {type}")
            };
        }

        public static InternalFormat ToGlInternalFormat(this TokPixelFormat type)
        {
            // Apparently there is no R5G6B5 format here.....

            return type switch
            {
                TokPixelFormat.FormatA8 => InternalFormat.R8,
                TokPixelFormat.FormatR5G6B5 => InternalFormat.Rgb,
                TokPixelFormat.FormatR5G5B5A1 => InternalFormat.Rgb5A1,
                TokPixelFormat.FormatR8G8B8 => InternalFormat.Rgb8,
                TokPixelFormat.FormatR8G8B8A8 => InternalFormat.Rgba8,
                _ => throw new Exception($"Unknown PixelFormat: {type}")
            };
        }
    }
}
