using System;
using System.Collections.Generic;
using System.Linq;

namespace Tokamak.Hosting.Config
{
    public static class ConfigPath
    {
        public const char SEPARATOR = '.';

        /// <summary>
        /// Validates that a give key segment has a legal name.
        /// </summary>
        /// <param name="name">The name to check</param>
        /// <returns>True if valid, false if not.</returns>
        public static bool IsValidKey(string name) =>
            !String.IsNullOrWhiteSpace(name) &&
            name.Trim().All(c => Char.IsLetterOrDigit(c) || c == '_')
        ;

        /// <summary>
        /// Splits a key along the separator character.
        /// </summary>
        /// <param name="path">The path to split.</param>
        /// <returns>A list of strings split along the separator character.</returns>
        public static IEnumerable<string> Split(string path) =>
            path?.Split(SEPARATOR, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim().ToLower()) ?? []
        ;

        /// <summary>
        /// Sanitizes a key segment to have only valid characters.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string SanitizeKey(string key) =>
            new String(key?.Where(c => Char.IsLetterOrDigit(c) || c == '_').ToArray() ?? []).ToLower()
        ;

        /// <summary>
        /// Sanitizes a path, removing empty entries and white space.
        /// </summary>
        /// <param name="path">The path to sanitize</param>
        /// <returns>A string with all items sanitized and lower cased.</returns>
        public static string Sanitize(string path) =>
            Combine(path?.Split() ?? [])
        ;

        /// <summary>
        /// Validates that the given path is a valid one.
        /// </summary>
        /// <param name="path">The path to validate</param>
        /// <returns>True if the path is valid, false if it is not.</returns>
        public static bool IsValidPath(string path) =>
            !String.IsNullOrWhiteSpace(path) &&
            Split(path).All(IsValidKey)
        ;

        /// <summary>
        /// Combines various key segments into a single path.
        /// </summary>
        /// <param name="sections">A list of keys to combine into a path.</param>
        /// <returns>A combined path.</returns>
        public static string Combine(params string[] sections) =>
            String.Join(SEPARATOR, sections
                .Where(s => !String.IsNullOrWhiteSpace(s))
                .Select(SanitizeKey)
            );
    }
}
