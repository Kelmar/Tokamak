using System.Collections.Generic;

namespace Tokamak.Abstractions.Config
{
    public interface IConfigSection : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>
        /// Gets the root configuration for this section.
        /// </summary>
        IConfiguration Root { get; }

        /// <summary>
        /// The path of the configuration starting at the root.
        /// </summary>
        /// <remarks>
        /// For the root configuration this will be an empty string.
        /// Otherwise this represents the path to the current section.
        /// </remarks>
        string Path { get; }

        /// <summary>
        /// Get/Set value in the configuration
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string this[string path] { get; set; }

        /// <summary>
        /// Gets a list of all keys for this configuration.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Gets a list of all values for this configuration.
        /// </summary>
        IEnumerable<string> Values { get; }

        /// <summary>
        /// Get a value from the configuration.
        /// </summary>
        /// <param name="path">Path is relative to this section.</param>
        /// <returns></returns>
        string Get(string path);

        /// <summary>
        /// Set a value in the section.
        /// </summary>
        /// <param name="path">Path is relative to this section.</param>
        /// <param name="value"></param>
        void Set(string path, string value);

        /// <summary>
        /// Gets a configuration subsection relative to this section.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IConfigSection GetSection(string path);
    }
}
