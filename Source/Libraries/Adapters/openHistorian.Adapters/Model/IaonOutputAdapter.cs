// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;
using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;

namespace openHistorian.Model
{
    public class IaonOutputAdapter
    {
        public Guid NodeID
        {
            get;
            set;
        }

        [PrimaryKey(true)]
        public int ID
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        [Label("Adapter Name")]
        [RegularExpression("^[A-Z0-9\\-!_\\.@#\\$]+$", ErrorMessage = "Only upper case letters, numbers, '!', '-', '@', '#', '_' , '.'and '$' are allowed.")]
        public string AdapterName
        {
            get;
            set;
        }

        [Required]
        [Label("Assembly Name")]
        public string AssemblyName
        {
            get;
            set;
        }

        [Required]
        [Label("Type Name")]
        public string TypeName
        {
            get;
            set;
        }

        [Label("Connection String")]
        public string ConnectionString
        {
            get;
            set;
        }
    }
}