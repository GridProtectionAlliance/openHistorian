// ReSharper disable CheckNamespace

using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;

#pragma warning disable 1591

namespace openHistorian.Model
{
    public class Runtime
    {
        [PrimaryKey(true)]
        public int ID { get; set; }

        public int SourceID { get; set; }

        [Required]
        [StringLength(200)]
        public string SourceTable { get; set; }
    }
}
