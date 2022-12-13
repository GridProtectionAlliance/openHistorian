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
    // DeviceGroupClass is simply a special OtherDevice record used to define device group classes with a long/lat of -999
    [PrimaryLabel("Acronym")]
    [TableName("OtherDevice")]
    public class DeviceGroupClass
    {
        public const decimal DefaultGeoID = -999.0M;
        
        [Label("Local Device ID")]
        [PrimaryKey(true)]
        public int ID { get; set; }

        [Required]
        [StringLength(200)]
        [AcronymValidation]
        public string Acronym { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [DefaultValue(false)]
        public bool IsConcentrator { get; set; }

        [Label("Company")]
        [DefaultValueExpression("Connection.ExecuteScalar(typeof(int), (object)null, 'SELECT ID FROM Company WHERE Acronym = {0}', Global.CompanyAcronym)", Cached = true)]
        public int? CompanyID { get; set; }

        [Label("Vendor Device")]
        public int? VendorDeviceID { get; set; }

        public decimal? Longitude { get; set; } = DefaultGeoID;

        public decimal? Latitude { get; set; } = DefaultGeoID;

        [Label("Interconnection")]
        [InitialValueScript("1")]
        public int? InterconnectionID { get; set; }

        [DefaultValue(false)]
        public bool Planned { get; set; }

        [DefaultValue(false)]
        public bool Desired { get; set; }

        [DefaultValue(false)]
        public bool InProgress { get; set; }

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