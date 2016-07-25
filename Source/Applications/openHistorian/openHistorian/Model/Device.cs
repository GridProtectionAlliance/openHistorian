using System;
using System.ComponentModel.DataAnnotations;
using GSF.Data.Model;

namespace openHistorian.Model
{
    public class Device
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

        public int? ParentID
        {
            get;
            set;
        }

        public Guid UniqueID
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        [RegularExpression("^[A-Z0-9\\-!_\\.@#\\$]+$", ErrorMessage = "Only upper case letters, numbers, '!', '-', '@', '#', '_' , '.'and '$' are allowed.")]
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

        [Label("Folder Name")]
        [StringLength(20)]
        public string OriginalSource
        {
            get;
            set;
        }

        public bool IsConcentrator
        {
            get;
            set;
        }

        [Required]
        [Label("Company")]
        public int? CompanyID
        {
            get;
            set;
        }

        public int? HistorianID
        {
            get;
            set;
        }

        public int AccessID
        {
            get;
            set;
        }

        [Label("Vendor Device")]
        public int? VendorDeviceID
        {
            get;
            set;
        }

        public int? ProtocolID
        {
            get;
            set;
        }

        public decimal? Longitude
        {
            get;
            set;
        }

        public decimal? Latitude
        {
            get;
            set;
        }

        public int? InterconnectionID
        {
            get;
            set;
        }

        public string ConnectionString
        {
            get;
            set;
        }

        [StringLength(200)]
        public string TimeZone
        {
            get;
            set;
        }

        public int? FramesPerSecond
        {
            get;
            set;
        }

        public long TimeAdjustmentTicks
        {
            get;
            set;
        }

        public double DataLossInterval
        {
            get;
            set;
        }

        public int AllowedParsingExceptions
        {
            get;
            set;
        }

        public double ParsingExceptionWindow
        {
            get;
            set;
        }

        public double DelayedConnectionInterval
        {
            get;
            set;
        }

        public bool AllowUseOfCachedConfiguration
        {
            get;
            set;
        }

        public bool AutoStartDataParsingSequence
        {
            get;
            set;
        }

        public bool SkipDisableRealTimeData
        {
            get;
            set;
        }

        public int MeasurementReportingInterval
        {
            get;
            set;
        }

        public bool ConnectOnDemand
        {
            get;
            set;
        }

        public string ContactList
        {
            get;
            set;
        }

        public int? MeasuredLines
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

        public DateTime CreatedOn
        {
            get;
            set;
        }

        [Required]
        [StringLength(50)]
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
        [StringLength(50)]
        public string UpdatedBy
        {
            get;
            set;
        }
    }
}