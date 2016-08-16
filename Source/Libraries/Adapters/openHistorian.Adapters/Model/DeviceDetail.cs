// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;

namespace openHistorian.Model
{
    public class DeviceDetail
    {
        public Guid NodeID { get; set; }
        public Guid UniqueID { get; set; }
        public string OriginalSource { get; set; }
        public bool IsConcentrator { get; set; }
        public string Acronym { get; set; }
        public string Name { get; set; }
        public int AccessID { get; set; }
        public string ParentAcronym { get; set; }
        public string ProtocolName { get; set; }
        public int FramesPerSecond { get; set; }
        public string CompanyAcronym { get; set; }
        public string VendorAcronym { get; set; }
        public string VendorDeviceName { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string InterconnectionName { get; set; }
        public string ContactList { get; set; }
        public bool Enabled { get; set; }
        public DateTime UpdatedOn { get; set; }

    }
}
