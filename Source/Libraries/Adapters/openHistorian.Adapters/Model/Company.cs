// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.ComponentModel.DataAnnotations;
using GSF.ComponentModel.DataAnnotations;
using GSF.ComponentModel;
using GSF.Data.Model;

namespace openHistorian.Model
{
    public class Company
    {
        private string m_mapAcronym;

        [PrimaryKey(true)]
        public int ID
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        [AcronymValidation]
        [Searchable]
        public string Acronym
        {
            get;
            set;
        }

        [Required]
        [StringLength(10)]
        [RegularExpression("^[A-Z0-9]+$", ErrorMessage = "Only three upper case letters or numbers are allowed.")]
        public string MapAcronym
        {
            get
            {
                return m_mapAcronym;
            }
            set
            {
                m_mapAcronym = value?.Trim();
            }
        }

        [Required]
        [StringLength(200)]
        [Searchable]
        public string Name
        {
            get;
            set;
        }

        public string URL
        {
            get;
            set;
        }

        public int LoadOrder
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