using System;
using System.Collections.Generic;
using System.Text;

namespace Tokamak.Readers.FBX
{
    internal record ImportResult
    {
        /// <summary>
        /// The internal ID for the importer.
        /// </summary>
        public required long InternalId { get; init; }

        /// <summary>
        /// The resulting imported name.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// The type of resource imported.
        /// </summary>
        public required ImportType ResourceType { get; init; }

        /// <summary>
        /// The read in object
        /// </summary>
        public required Object Result { get; init; }
    }

    internal enum ImportType
    {
        Texture,
        Material,
        Mesh,
        Skeleton,
        Model
    }
}
