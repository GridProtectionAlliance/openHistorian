using System;
using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;

namespace openHistorian.Model
{
    public class Measurement
    {
        [PrimaryKey(true)]
        public int PointID
        {
            get;
            set;
        }

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

        [Required]
        [StringLength(200)]
        public string PointTag
        {
            get;
            set;
        }

        public string AlternateTag
        {
            get;
            set;
        }

        public int SignalTypeID
        {
            get;
            set;
        }

        public int? PhasorSourceIndex
        {
            get;
            set;
        }

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