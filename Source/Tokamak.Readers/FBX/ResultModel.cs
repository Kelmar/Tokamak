using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Tokamak.Tritium.Rendering;

namespace Tokamak.Readers.FBX
{
    internal class ResultModel : ResultRecord
    {
        public ResultModel()
            : base(ImportType.Model)
        {
        }

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
