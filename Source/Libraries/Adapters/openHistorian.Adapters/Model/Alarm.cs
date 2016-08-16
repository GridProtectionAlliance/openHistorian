// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.ComponentModel.DataAnnotations;

namespace openHistorian.Model
{
    public class Alarm
    {
        public Guid NodeID
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string TagName
        {
            get;
            set;
        }

        public Guid SignalID
        {
            get;
            set;
        }

        public Guid? AssociatedMeasurementID
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public int Severity
        {
            get;
            set;
        }

        public int Operation
        {
            get;
            set;
        }

        public double? SetPoint
        {
            get;
            set;
        }

        public double? Tolerance
        {
            get;
            set;
        }

        public double? Delay
        {
            get;
            set;
        }

        public double? Hysteresis
        {
            get;
            set;
        }

        public int LoadOrder
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