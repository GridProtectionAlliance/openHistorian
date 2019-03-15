// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System.Collections.Generic;

namespace openHistorian.Adapters
{
    public class FrequencyDefinition
    {
        public string Label { get; set; }
    }

    public class PhasorDefinition
    {
        public string Label { get; set; }

        public string PhasorType { get; set; }
    }

    public class AnalogDefinition
    {
        public string Label { get; set; }

        public string AnalogType { get; set; }
    }

    public class DigitalDefinition
    {
        public string Label { get; set; }
    }

    public class ConfigurationCell
    {
        public ushort IDCode { get; set; }

        public string StationName { get; set; }

        public string IDLabel { get; set; }

        public FrequencyDefinition FrequencyDefinition { get; set; }

        public List<PhasorDefinition> PhasorDefinitions { get; } = new List<PhasorDefinition>();

        public List<AnalogDefinition> AnalogDefinitions { get; } = new List<AnalogDefinition>();

        public List<DigitalDefinition> DigitalDefinitions { get; } = new List<DigitalDefinition>();
    }

    public class ConfigurationFrame
    {
        public List<ConfigurationCell> Cells { get; } = new List<ConfigurationCell>();

        public ushort IDCode { get; set; }

        public ushort FrameRate { get; set; }

        public string ConnectionString { get; set; }

        public int ProtocolID { get; set; }
    }
}