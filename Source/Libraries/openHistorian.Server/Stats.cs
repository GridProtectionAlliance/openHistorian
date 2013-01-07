using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openHistorian
{
    public static class Stats
    {
        public static long PointsReturned;
        public static long PointsScanned;
        public static long QueriesExecuted;
        public static long SeeksRequested;
        public static void Clear()
        {
            PointsReturned = 0;
            PointsScanned = 0;
            QueriesExecuted = 0;
            SeeksRequested = 0;
        }

    }
}
