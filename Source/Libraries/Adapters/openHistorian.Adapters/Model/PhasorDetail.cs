// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;

namespace openHistorian.Model
{
    public class PhasorDetail
    {
        public string DeviceAcronym { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string Phase { get; set; }
        public int SourceIndex { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
