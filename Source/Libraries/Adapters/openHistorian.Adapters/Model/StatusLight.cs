// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;

namespace openHistorian.Model
{
    public class StatusLight
    {
        public string DeviceAcronym { get; set; }
        public DateTime Timestamp { get; set; }
        public bool GoodData { get; set; }
    }
}
