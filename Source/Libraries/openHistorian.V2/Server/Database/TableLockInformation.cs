using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.V2.Server.Database
{
    class TableLockInformation
    {
        public long ReadLockCount;
        public TableSummaryInfo Table;

    }
}
