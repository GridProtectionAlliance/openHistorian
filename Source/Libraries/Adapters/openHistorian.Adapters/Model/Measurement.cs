// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.ComponentModel.DataAnnotations;
using GSF.ComponentModel.DataAnnotations;
using GSF.Data.Model;

namespace openHistorian.Model
{
    public class Measurement
    {
        [Label("Point ID")]
        [PrimaryKey(true)]
        [Searchable]
        public int PointID
        {
            get;
            set;
        }

        [Label("Unique Signal ID")]
        [Searchable]
        public Guid SignalID
        {
            get;
            set;
        }

        public int? HistorianID
        {
            get;
            set;
        }

        public int? DeviceID
        {
            get;
            set;
        }

        [Label("Tag Name")]
        [Required]
        [StringLength(200)]
        [Searchable]
        public string PointTag
        {
            get;
            set;
        }

        [Label("Alternate Tag Name")]
        [Searchable]
        public string AlternateTag
        {
            get;
            set;
        }

        [Label("Signal Type")]
        public int SignalTypeID
        {
            get;
            set;
        }

        [Label("Phasor Source Index")]
        public int? PhasorSourceIndex
        {
            get;
            set;
        }

        [Label("Signal Reference")]
        [Required]
        [StringLength(200)]
        public string SignalReference
        {
            get;
            set;
        }

        public double Adder
        {
            get;
            set;
        }

        public double Multiplier
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public bool Internal
        {
            get;
            set;
        }

        public bool Subscribed
        {
            get;
            set;
        }

        public bool Enabled
        {
            get;
            set;
        }

        public DateTime CreatedOn
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string CreatedBy
        {
            get;
            set;
        }

        public DateTime UpdatedOn
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string UpdatedBy
        {
            get;
            set;
        }
    }
}