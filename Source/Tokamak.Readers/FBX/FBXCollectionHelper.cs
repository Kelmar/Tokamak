using System;
using System.Collections.Generic;
using System.Linq;

using Tokamak.Readers.FBX.DOM;

namespace Tokamak.Readers.FBX
{
    internal static class FBXCollectionHelper
    {
        /// <summary>
        /// Fetch a list of node IDs who's type of the given type.
        /// </summary>
        /// <param name="type">The type of node to search for.</param>
        /// <returns>An enumeration of IDs that can be parsed.</returns>
        public static IEnumerable<Node> WithFBXType(this IEnumerable<Node> nodes, string type)
            => nodes.Where(n => StringComparer.OrdinalIgnoreCase.Equals(n.Name, type));

        public static IEnumerable<FBXObject> WithFBXType(this IEnumerable<FBXObject> objects, string type)
            => objects.Where(o => StringComparer.OrdinalIgnoreCase.Equals(o.Type, type));

        public static IEnumerable<Node> WithName(this IEnumerable<Node> nodes, string name)
            => nodes.Where(n => StringComparer.OrdinalIgnoreCase.Equals(n.Name, name));

        public static Node? FirstWithName(this IEnumerable<Node> nodes, string name)
            => nodes.FirstOrDefault(n => StringComparer.OrdinalIgnoreCase.Equals(n.Name, name));

        public static T? FirstWithName<T>(this IEnumerable<T> objects, string name)
            where T : FBXObject
            => objects.FirstOrDefault(o => StringComparer.OrdinalIgnoreCase.Equals(o.Name, name));
    }
}
