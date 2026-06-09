using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;

using Tokamak.Utilities;

namespace Tokamak.Readers.FBX.DOM
{
    /// <summary>
    /// Parsed Properties70 of an object node.
    /// </summary>
    internal class ObjectProperty
    {
        private ObjectProperty(Node node, string name, string type, object data)
        {
            Node = node;
            Name = name;
            Type = type;
            Data = data;
        }

        public Node Node { get; }

        public string Name { get; }

        public string Type { get; }

        public object Data { get; }

        public override string ToString() => $"{Name}: {Type}";

        public static ObjectProperty? Build(Node node)
        {
            string name = node.StringProperty(0);
            string type = node.StringProperty(1);

            object? data = ReadData(node, type);

            if (data == null)
                return null;

            return new ObjectProperty(node, name, type, data);
        }

        public static IEnumerable<ObjectProperty> BuildAllFor(Node node)
        {
            return node.Children
                .WithName("Properties70")
                .SelectMany(p => p.Children.WithName("P"))
                .Select(Build)
                .NotNull();
        }

        private static object? ReadData(Node node, string type)
        {
            /*
             * I suspect these could be just about anything.  As such, I'm
             * wondering if there's a better more generic way to handle this.
             * 
             *          -- B.Simonds (May 27, 2026)
             */

            return type.ToLower() switch
            {
                "bool" => ReadBool(node),
                "enum" => ReadNumber(node),
                "int" => ReadNumber(node),
                "short" => ReadNumber(node),
                "long" => ReadNumber(node),
                "float" => ReadNumber(node),
                "double" => ReadNumber(node),
                "number" => ReadNumber(node),
                "string" => ReadString(node),
                "kstring" => ReadString(node),
                "colorrgb" => ReadVector3(node),
                "color" => ReadVector4(node),
                "vector2d" => ReadVector2(node),
                "vector3d" => ReadVector3(node),
                "vector4d" => ReadVector4(node),
                "lcl translation" => ReadVector3(node),
                "lcl rotation" => ReadVector3(node),
                "lcl scaling" => ReadVector3(node),
                _ => null
            };
        }

        /*
         * TODO: Want to rework how we handle these properties entirely.
         *
         * The current system is somewhat fragile and apparently has some boxing/unboxing
         * issues that could stand to be resolved.
         * 
         * Our current method of trying to start from the end of the property list and
         * read from there to get values also suffers from the potential problem of 
         * possibly trying to start out with negative indies.  While the ReadValues()
         * method sort of guards against this, it still probably isn't terribly
         * great that this happens.
         */

        private static bool ReadBool(Node node)
        {
            int idx = node.Properties.Count - 1;
            return node.Properties[idx].AsBool();
        }

        private static object ReadNumber(Node node)
        {
            int idx = node.Properties.Count - 1;
            return node.Properties[idx].Data;
        }

        private static object ReadString(Node node)
        {
            int idx = node.Properties.Count - 1;
            return node.Properties[idx].Data.ToString() ?? String.Empty;
        }

        private static IEnumerable<float> ReadValues(Node node, int idx, int count)
        {
            idx = Math.Max(idx, 0);

            for (int produced = 0; produced < count; ++idx)
            {
                if (idx >= node.Properties.Count)
                {
                    // Pad with zeros after reaching end...
                    ++produced;
                    yield return 0;
                    continue;
                }

                NodeProperty prop = node.Properties[idx];

                if (!prop.Type.IsNumeric)
                    continue; // Skip non-numeric value.

                ++produced;
                yield return Convert.ToSingle(prop.Data);
            }
        }

        private static Vector2 ReadVector2(Node node)
        {
            int idx = node.Properties.Count - 2;
            var value = ReadValues(node, idx, 2).ToArray();
            return VectorEx.ToVector2(value);
        }

        private static Vector3 ReadVector3(Node node)
        {
            int idx = node.Properties.Count - 3;
            var value = ReadValues(node, idx, 3).ToArray();
            return VectorEx.ToVector3(value);
        }

        private static Vector4 ReadVector4(Node node)
        {
            int idx = node.Properties.Count - 4;
            var value = ReadValues(node, idx, 4).ToArray();
            return VectorEx.ToVector4(value);
        }
    }
}
