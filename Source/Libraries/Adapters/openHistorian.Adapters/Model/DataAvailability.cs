using GSF.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openHistorian.Adapters.Model
{
    /// <summary>
    /// Data Availability 
    /// </summary>
    public class DataAvailability
    {
        /// <summary>
        /// Data Availability 
        /// </summary>
        [PrimaryKey(true)]
        public int ID { get; set; }
        /// <summary>
        /// Data Availability 
        /// </summary>
        public float GoodAvailableData { get; set; }
        /// <summary>
        /// Data Availability 
        /// </summary>
        public float BadAvailableData { get; set; }
        /// <summary>
        /// Data Availability 
        /// </summary>
        public float TotalAvailableData { get; set; }
    }
}
