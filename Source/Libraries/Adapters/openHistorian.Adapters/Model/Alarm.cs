// ReSharper disable CheckNamespace
#pragma warning disable 1591

using GSF.ComponentModel;
using GSF.Data.Model;
using GSF.Security;
using System;
using System.ComponentModel.DataAnnotations;

namespace openHistorian.Model
{
    public class Alarm
    {
        static Alarm()
        {
            TableOperations<Alarm>.TypeRegistry.RegisterType<AdoSecurityProvider>();
        }

        [DefaultValueExpression("AdoSecurityProvider.DefaultNodeID", Cached = true)]
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