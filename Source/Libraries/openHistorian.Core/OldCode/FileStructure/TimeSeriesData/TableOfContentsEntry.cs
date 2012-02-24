using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian.FileStructure.TimeSeriesData
{
    struct TableOfContentsEntry
    {
        Guid PointGuid;
        int PointID;
        DateTime StartTime;
        DateTime EndTime;
    }
}
