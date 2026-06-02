using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tokamak.Readers.FBX
{
    internal abstract class ResultRecord
    {
        protected ResultRecord(ImportType type) { Type = type; }

        [NotMapped]
        public long Id { get; set; }

        [NotMapped]
        public string Name { get; set; } = String.Empty;

        [NotMapped]
        public ImportType Type { get; }
    }
}
