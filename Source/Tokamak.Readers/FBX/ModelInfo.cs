using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Tokamak.Tritium.Rendering;

namespace Tokamak.Readers.FBX
{
    internal class ModelInfo : ResultRecord
    {
        public ModelInfo()
            : base(ImportType.Model)
        {
        }

        public long ParentId { get; set; } = 0;

        [NotMapped]
        public List<long> MaterialIds
        {
            get;
            set => field = value ?? [];
        } = [];

        [NotMapped]
        public List<long> MeshIds
        {
            get;
            set => field = value ?? [];
        } = [];
    }
}
