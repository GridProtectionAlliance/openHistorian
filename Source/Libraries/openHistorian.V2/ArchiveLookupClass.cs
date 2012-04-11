using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.V2
{
    class ArchiveLookupClass
    {
        ArchiveLookupSnapshot m_activeSnapshot;

        public ArchiveLookupClass()
        {
            m_activeSnapshot = new ArchiveLookupSnapshot();
        }

        public ArchiveLookupSnapshot GetLatestSnapshot()
        {
            lock (this)
            {
                return m_activeSnapshot;
            }
        }




    }
}
