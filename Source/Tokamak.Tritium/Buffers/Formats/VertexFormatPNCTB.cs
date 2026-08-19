using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Tokamak.Tritium.Buffers.Formats;

/// <summary>
/// Format containing vertex positional info, vertex normals, color, texture coordinates, and bone info.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal record struct VertexFormatPNCTB
{
    /// <summary>
    /// Vertex position in 3D space.
    /// </summary>
    [FormatDescriptor(FormatBaseType.Float, 3)]
    public Vector3 Point;

    /// <summary>
    /// Vertex normal information for lighting calculations.
    /// </summary>
    [FormatDescriptor(FormatBaseType.Float, 3)]
    public Vector3 Normal;

    /// <summary>
    /// Color with alpha blending.
    /// </summary>
    /// <remarks>
    /// Values are floating point from 0 to 1.
    /// </remarks>
    [FormatDescriptor(FormatBaseType.Float, 4)]
    public Vector4 Color;

    /// <summary>
    /// UV coordinates for texture mapping.
    /// </summary>
    [FormatDescriptor(FormatBaseType.Float, 2)]
    public Vector2 TexCoord;

    /// <summary>
    /// Bone indices, using UInt64 as array of four UInt16s
    /// </summary>
    [FormatDescriptor(FormatBaseType.UnsignedShort, 4)]
    public UInt64 BoneIndices;

    /// <summary>
    /// Bone weights, using Vector4 as an array.
    /// </summary>
    [FormatDescriptor(FormatBaseType.Float, 4)]
    public Vector4 BoneWeights;
}