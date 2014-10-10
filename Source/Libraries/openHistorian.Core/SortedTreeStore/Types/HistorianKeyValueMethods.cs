using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.SortedTreeStore.Tree;
using openHistorian.Collections;

namespace openHistorian.SortedTreeStore.Types
{
    public class HistorianKeyValueMethods 
        : KeyValueMethods<HistorianKey, HistorianValue>
    {
        public override void Copy(HistorianKey srcKey, HistorianValue srcValue, HistorianKey destKey, HistorianValue dstValue)
        {
            destKey.Timestamp = srcKey.Timestamp;
            destKey.PointID = srcKey.PointID;
            destKey.EntryNumber = srcKey.EntryNumber;
            dstValue.Value1 = srcValue.Value1;
            dstValue.Value2 = srcValue.Value2;
            dstValue.Value3 = srcValue.Value3;
        }
    }
}
