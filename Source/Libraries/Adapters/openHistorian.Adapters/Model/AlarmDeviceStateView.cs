using GSF.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openHistorian.Adapters.Model
{
    /// <summary>
    /// Will be used to return this view to grafana
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
