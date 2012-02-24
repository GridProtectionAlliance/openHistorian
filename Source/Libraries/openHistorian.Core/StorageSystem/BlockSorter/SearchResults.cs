using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian.StorageSystem.BlockSorter
{
    public enum SearchResults
    {
        StartOfExactMatch,
        RightAfterClosestMatchWithoutGoingOver,
        StartOfEndOfStream
    }
}
