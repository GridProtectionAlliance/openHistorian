// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;

namespace openHistorian.Model
{
    public class MeasurementDetail
    {
        public string DeviceAcronym { get; set; }
        public string ID { get; set; }
        public Guid SignalID { get; set; }
        public string PointTag { get; set; }
        public string SignalReference { get; set; }
        public string SignalAcronym { get; set; }
        public int PhasorSourceIndex { get; set; }
        public string Description { get; set; }
        public bool Internal { get; set; }
        public bool Enabled { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
