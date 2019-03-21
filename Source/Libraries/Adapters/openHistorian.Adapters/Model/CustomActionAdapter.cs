// ReSharper disable CheckNamespace

#pragma warning disable 1591

using GSF.ComponentModel;
using GSF.ComponentModel.DataAnnotations;
using GSF.Data.Model;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace openHistorian.Model
{
    public class CustomActionAdapter
    {
        // TODO: This expression fails to evaluate, even when Global is defined
        //[DefaultValueExpression("Global.NodeID")]
        public Guid NodeID { get; set; }

        [PrimaryKey(true)]
        public int ID { get; set; }

        [Required]
        [StringLength(200)]
        [AcronymValidation]
        public string AdapterName { get; set; }

        [Required]
        [DefaultValue("DynamicCalculator.dll")]
        public string AssemblyName { get; set; }

        [Required]
        [DefaultValue("DynamicCalculator.DynamicCalculator")]
        public string TypeName { get; set; }

        public string ConnectionString { get; set; }

        public int LoadOrder { get; set; }

        public bool Enabled { get; set; }

        [DefaultValueExpression("DateTime.UtcNow")]
        public DateTime CreatedOn { get; set; }

        [Required]
        [StringLength(200)]
        [DefaultValueExpression("UserInfo.CurrentUserID")]
        public string CreatedBy { get; set; }

        [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
        [UpdateValueExpression("DateTime.UtcNow")]
        public DateTime UpdatedOn { get; set; }

        [Required]
        [StringLength(200)]
        [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
        [UpdateValueExpression("UserInfo.CurrentUserID")]
        public string UpdatedBy { get; set; }
    }
}