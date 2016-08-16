// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.ComponentModel.DataAnnotations;

namespace openHistorian.Model
{
    public class Historian
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
        public string Acronym
        {
            get;
            set;
        }

        [StringLength(200)]
        public string Name
        {
            get;
            set;
        }

        public string AssemblyName
        {
            get;
            set;
        }

        public string TypeName
        {
            get;
            set;
        }

        public string ConnectionString
        {
            get;
            set;
        }

        public bool IsLocal
        {
            get;
            set;
        }

        public int MeasurementReportingInterval
        {
            get;
            set;
        }

        public string Description
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