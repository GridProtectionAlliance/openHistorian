// ReSharper disable CheckNamespace
#pragma warning disable 1591

using GSF.ComponentModel;
using GSF.Data.Model;
using System;

namespace openHistorian.Adapters
{
    public class PowerCalculation
    {
        [DefaultValueExpression("Global.NodeID")]
        public Guid NodeID { get; set; }

        [PrimaryKey(true)]
        public int ID { get; set; }

        public string CircuitDescription { get; set; }

        public Guid VoltageAngleSignalID { get; set; }

        public Guid VoltageMagSignalID { get; set; }

        public Guid CurrentAngleSignalID { get; set; }

        public Guid CurrentMagSignalID { get; set; }

        public Guid? ActivePowerOutputSignalID { get; set; }

        public Guid? ReactivePowerOutputSignalID { get; set; }

        public Guid? ApparentPowerOutputSignalID { get; set; }

        public bool Enabled { get; set; }
    }
}
