// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;

namespace openHistorian.Model
{
    public class SignalType
    {
        [PrimaryKey(true)]
        public int ID
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string Name
        {
            get;
            set;
        }

        [Required]
        [StringLength(4)]
        public string Acronym
        {
            get;
            set;
        }

        [Required]
        [StringLength(2)]
        public string Suffix
        {
            get;
            set;
        }

        [Required]
        [StringLength(2)]
        public string Abbreviation
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string LongAcronym
        {
            get;
            set;
        }

        [Required]
        [StringLength(10)]
        public string Source
        {
            get;
            set;
        }

        [StringLength(10)]
        public string EngineeringUnits
        {
            get;
            set;
        }
    }
}