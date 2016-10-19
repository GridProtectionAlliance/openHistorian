// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.ComponentModel.DataAnnotations;
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
        [RegularExpression("^[A-Z0-9\\-!_\\.@#\\$]+$", ErrorMessage = "Only upper case letters, numbers, '!', '-', '@', '#', '_' , '.'and '$' are allowed.")]
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