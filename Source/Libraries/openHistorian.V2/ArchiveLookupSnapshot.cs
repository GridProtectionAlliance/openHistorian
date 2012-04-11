using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.V2
{
    class ArchiveLookupSnapshot
    {
        public List<TableSummaryInfo> ArchiveTables;

        public ArchiveLookupSnapshot()
        {
            ArchiveTables = new List<TableSummaryInfo>();
        }

        public ArchiveLookupSnapshot Clone()
        {
            ArchiveLookupSnapshot snapshot = new ArchiveLookupSnapshot();
            snapshot.ArchiveTables.AddRange(ArchiveTables);
            return snapshot;
        }
    }
}
