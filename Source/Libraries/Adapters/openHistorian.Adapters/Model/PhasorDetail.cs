// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.ComponentModel.DataAnnotations;
using GSF.ComponentModel;
using GSF.Data.Model;

namespace openHistorian.Model
{
    public class PhasorDetail
    {
        [PrimaryKey(true)]
        public int ID { get; set; }

        public int DeviceID { get; set; }

        public string DeviceAcronym { get; set; }

        public string Label { get; set; }

        public string Type { get; set; }

        public string Phase { get; set; }

        public int SourceIndex { get; set; }

        public int? DestinationPhasorID { get; set; }

        public string DestinationPhasorLabel { get; set; }

        public int BaseKV { get; set; }

        [DefaultValueExpression("DateTime.UtcNow")]
        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(50)]
        [DefaultValueExpression("UserInfo.CurrentUserID")]
        public string CreatedBy { get; set; }

        [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
        [UpdateValueExpression("DateTime.UtcNow")]
        public DateTime UpdatedOn { get; set; }

        [Required]
        [StringLength(50)]
        [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
        [UpdateValueExpression("UserInfo.CurrentUserID")]
        public string UpdatedBy { get; set; }
    }
}
