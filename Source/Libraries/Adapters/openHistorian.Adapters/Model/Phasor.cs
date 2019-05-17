// ReSharper disable CheckNamespace

#pragma warning disable 1591

using GSF.ComponentModel;
using GSF.ComponentModel.DataAnnotations;
using GSF.Data.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace openHistorian.Model
{
    [PrimaryLabel("Label")]
    public class Phasor
    {
        [PrimaryKey(true)]
        public int ID { get; set; }

        public int DeviceID { get; set; }

        [Required]
        [StringLength(200)]
        public string Label { get; set; }

        public char Type { get; set; }

        public char Phase { get; set; }

        public int BaseKV { get; set; }

        public int? DestinationPhasorID { get; set; }

        public int SourceIndex { get; set; }

        /// <summary>
        ///     Created on field.
        /// </summary>
        [DefaultValueExpression("DateTime.UtcNow")]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        ///     Created by field.
        /// </summary>
        [Required]
        [StringLength(50)]
        [DefaultValueExpression("UserInfo.CurrentUserID")]
        public string CreatedBy { get; set; }

        /// <summary>
        ///     Updated on field.
        /// </summary>
        [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
        [UpdateValueExpression("DateTime.UtcNow")]
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        ///     Updated by field.
        /// </summary>
        [Required]
        [StringLength(50)]
        [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
        [UpdateValueExpression("UserInfo.CurrentUserID")]
        public string UpdatedBy { get; set; }
    }
}