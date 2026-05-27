using System.Collections.Generic;
using System.Linq;

namespace Tokamak.Utilities
{
    public static class CollectionHelpers
    {
        /// <summary>
        /// Applies a set of changes from the source dictionary to the target dictionary.
        /// </summary>
        /// <typeparam name="TKey">The key type of the dictionaries</typeparam>
        /// <typeparam name="TValue">The value type of the dictionaries</typeparam>
        /// <param name="target">The target dictionary to update.</param>
        /// <param name="source">The source dictionary to get updates from.</param>
        public static void Apply<TKey, TValue>(this IDictionary<TKey, TValue> target, IDictionary<TKey, TValue> source)
        {
            foreach (var kvp in source)
                target[kvp.Key] = kvp.Value;
        }

        /// <summary>
        /// Applies a set of changes from the source dictionary to the target dictionary.
        /// </summary>
        /// <typeparam name="TKey">The key type of the dictionaries</typeparam>
        /// <typeparam name="TValue">The value type of the dictionaries</typeparam>
        /// <param name="target">The target dictionary to update.</param>
        /// <param name="source">The source dictionary to get updates from.</param>
        public static void Apply<TKey, TValue>(this IDictionary<TKey, TValue> target, IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            foreach (var kvp in source)
                target[kvp.Key] = kvp.Value;
        }

        /// <summary>
        /// Filter null values out of a collection, returning a collection of non-null values.
        /// </summary>
        /// <typeparam name="TValue">The value type to filter on.</typeparam>
        /// <param name="values">The collection of possibly null values to filter.</param>
        public static IEnumerable<TValue> NotNull<TValue>(this IEnumerable<TValue?> values)
            => values.Where(v => v != null).Select(v => v!);

        /// <summary>
        /// Flatten a lookup into a single enumerable collection.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="lookup"></param>
        /// <returns></returns>
        public static IEnumerable<TValue> Flatten<TKey, TValue>(this ILookup<TKey, TValue> lookup)
        {
            foreach (var grp in lookup)
            {
                foreach (var item in lookup[grp.Key])
                    yield return item;
            }
        }
    }
}
