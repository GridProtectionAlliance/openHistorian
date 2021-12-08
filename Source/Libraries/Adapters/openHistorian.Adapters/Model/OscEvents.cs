// ReSharper disable CheckNamespace
#pragma warning disable 1591

using GSF.ComponentModel.DataAnnotations;
using GSF.Data.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace openHistorian.Model
{
    public class OscEvents
    {
        [PrimaryKey(true)]
        public int ID { get; set; }

        [Label("Oscillation Source")]
        [Required]
        [StringLength(200)]
        [Searchable]
        public string Source { get; set; }

        [Label("Start Time")]
        public DateTime? StartTime { get; set; }

        [Label("Stop Time")]
        public DateTime? StopTime { get; set; }

        [StringLength(200)]
        public string Frequency { get; set; }

        [StringLength(200)]
        public string Magnitude { get; set; }

        [Searchable]
        public string Notes { get; set; }
    }
}
