// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;
using GSF.Web.Model;

namespace openHistorian.Model
{
    public class VendorDevice
    {
        [PrimaryKey(true)]
        public int ID
        {
            get;
            set;
        }

        public int VendorID
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        [Searchable]
        public string Name
        {
            get;
            set;
        }

        [Searchable]
        public string Description
        {
            get;
            set;
        }

        [Label("Web Page")]
        [RegularExpression(DataContext.UrlValidation, ErrorMessage = "Invalid URL.")]
        public string URL
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