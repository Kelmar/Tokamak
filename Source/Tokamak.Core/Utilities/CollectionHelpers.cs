using System.Collections.Generic;

namespace Tokamak.Core.Utilities
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
    }
}
