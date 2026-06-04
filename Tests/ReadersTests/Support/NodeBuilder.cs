using System.Collections.Generic;
using System.Linq;

using Tokamak.Readers.FBX.DOM;

namespace ReadersTests.Support
{
    /// <summary>
    /// Helpers for hand-building <see cref="Node"/> / <see cref="NodeProperty"/> trees
    /// so the DOM/mapper logic can be exercised without a real FBX file.
    /// </summary>
    internal static class NodeBuilder
    {
        public static NodeProperty Prop(PropertyType type, object data)
            => new NodeProperty { Type = type, Data = data };

        public static NodeProperty Str(string value) => Prop(PropertyType.String, value);

        public static NodeProperty Int(int value) => Prop(PropertyType.SignedInt, value);

        public static NodeProperty Long(long value) => Prop(PropertyType.SignedLong, value);

        public static NodeProperty Dbl(double value) => Prop(PropertyType.Double, value);

        public static NodeProperty IntArray(params int[] values)
            => Prop(PropertyType.IntArray, values);

        public static NodeProperty FloatArray(params float[] values)
            => Prop(PropertyType.FloatArray, values);

        public static NodeProperty DoubleArray(params double[] values)
            => Prop(PropertyType.DoubleArray, values);

        public static Node Make(string name, IEnumerable<NodeProperty> props = null, IEnumerable<Node> children = null)
        {
            return new Node
            {
                Name = name,
                Properties = props?.ToList() ?? [],
                Children = children?.ToList() ?? []
            };
        }

        /// <summary>
        /// Builds a single string-valued child node (e.g. MappingInformationType / ReferenceInformationType).
        /// </summary>
        public static Node StringNode(string name, string value)
            => Make(name, [ Str(value) ]);
    }
}
