// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System;

namespace openHistorian.Model
{
    public class MeasurementValue
    {
        public Double Timestamp { get; set; }
        public Double Value { get; set; }
        public Guid ID { get; set; }
    }
}
