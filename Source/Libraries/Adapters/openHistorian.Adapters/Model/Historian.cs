// ReSharper disable CheckNamespace
#pragma warning disable 1591

using GSF.ComponentModel;
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

        /// <summary>
        /// Created on field.
        /// </summary>
        [DefaultValueExpression("DateTime.UtcNow")]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Created by field.
        /// </summary>
        [Required]
        [StringLength(200)]
        [DefaultValueExpression("UserInfo.CurrentUserID")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Updated on field.
        /// </summary>
        [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
        [UpdateValueExpression("DateTime.UtcNow")]
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// Updated by field.
        /// </summary>
        [Required]
        [StringLength(200)]
        [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
        [UpdateValueExpression("UserInfo.CurrentUserID")]
        public string UpdatedBy { get; set; }
    }
}