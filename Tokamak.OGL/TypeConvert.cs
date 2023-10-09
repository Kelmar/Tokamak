using System;

using OpenTK.Graphics.OpenGL4;

using GLPrimType = OpenTK.Graphics.OpenGL4.PrimitiveType;
using TPrimType = Tokamak.PrimitiveType;
using FormatType = Tokamak.Formats.FormatBaseType;
using Tokamak.Buffer;

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
                TPrimType.TrangleList => GLPrimType.Triangles,
                TPrimType.TrangleStrip => GLPrimType.TriangleStrip,
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

        public static BufferUsageHint ToGLType(this BufferType type)
        {
            return type switch
            {
                BufferType.Static => BufferUsageHint.StaticDraw,
                BufferType.Dyanmic => BufferUsageHint.StreamDraw,
                BufferType.Volatile => BufferUsageHint.StaticDraw,
                BufferType.Immutable => BufferUsageHint.StaticDraw,
                _ => throw new Exception($"Unknown buffer type {type}")
            };
        }
    }
}
