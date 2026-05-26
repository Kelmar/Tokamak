using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Readers.FBX
{
    internal class CompoundProperty
    {
        private CompoundProperty(Node node, string name, string type, object data)
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

        public override string ToString() => $"{Name}: {Data}";

        public static CompoundProperty? Build(Node node)
        {
            string name = node.Properties[0].ToString();
            string type = node.Properties[1].ToString();

            object? data = ReadData(node, type);

            if (data == null)
                return null;

            return new CompoundProperty(node, name, type, data);
        }

        public static IEnumerable<CompoundProperty> BuildAllFor(Node node)
        {
            var props = node
                .GetChildren("Properties70")
                .SelectMany(p => p.GetChildren("P"))
                .Select(Build);

            foreach (var p in props)
            {
                if (p != null)
                    yield return p;
            }
        }

        private static object? ReadData(Node node, string type)
        {
            return type.ToLower() switch
            {
                "int" => ReadNumber(node),
                "float" => ReadNumber(node),
                "double" => ReadNumber(node),
                "number" => ReadNumber(node),
                "string" => ReadString(node),
                "kstring" => ReadString(node),
                "colorrgb" => ReadVector3(node),
                "color" => ReadVector4(node),
                _ => null
            };
        }

        private static object ReadNumber(Node node)
        {
            int idx = node.Properties.Count - 1;
            return node.Properties[idx].Data!;
        }

        private static object ReadString(Node node)
        {
            int idx = node.Properties.Count - 1;
            return node.Properties[idx].Data!.ToString() ?? String.Empty;
        }

        private static IEnumerable<float> ReadValues(Node node, int idx, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                if (node.Properties.Count >= (i + idx))
                    yield return 0;

                yield return Convert.ToSingle(node.Properties[i + idx].Data!);
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
