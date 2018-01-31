// ReSharper disable CheckNamespace
#pragma warning disable 1591

using GSF.Data.Model;

namespace openHistorian.Adapters.Model
{
    /// <summary>
    /// Defines fields for AlarmDeviceState view.
    /// </summary>
    public class AlarmDeviceStateView
    {
        [PrimaryKey(true)]
        public int ID { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public string Color { get; set; }
        public string DisplayData { get; set; }
    }
}
