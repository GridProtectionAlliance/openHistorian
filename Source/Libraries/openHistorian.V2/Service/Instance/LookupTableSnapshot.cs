using System.Collections.Generic;

namespace openHistorian.V2.Service.Instance
{
    class LookupTableSnapshot
    {
        public List<TableSummaryInfo> ArchiveTables;

        public LookupTableSnapshot()
        {
            ArchiveTables = new List<TableSummaryInfo>();
        }

        public LookupTableSnapshot Clone()
        {
            LookupTableSnapshot snapshot = new LookupTableSnapshot();
            snapshot.ArchiveTables.AddRange(ArchiveTables);
            return snapshot;
        }
    }
}
