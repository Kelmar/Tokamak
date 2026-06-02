using System;
using System.Collections.Generic;
using System.Linq;

using Tokamak.Readers.FBX.DOM;

namespace Tokamak.Readers.FBX.Builders
{
    internal static class FBXCollectionHelper
    {
        /// <summary>
        /// Fetch a list of node IDs who's type of the given type.
        /// </summary>
        /// <param name="type">The type of node to search for.</param>
        /// <returns>An enumeration of IDs that can be parsed.</returns>
        public static IEnumerable<Node> WithFBXType(this IEnumerable<Node> nodes, string type)
            => nodes.Where(n => StringComparer.InvariantCultureIgnoreCase.Equals(n.Name, type));

        public static IEnumerable<FBXObject> WithFBXType(this IEnumerable<FBXObject> objects, string type)
            => objects.Where(o => StringComparer.InvariantCultureIgnoreCase.Equals(o.Type, type));

        public static T? WithName<T>(this IEnumerable<T> objects, string name)
            where T : FBXObject
            => objects.FirstOrDefault(o => StringComparer.InvariantCultureIgnoreCase.Equals(o.Name, name));
    }
}
