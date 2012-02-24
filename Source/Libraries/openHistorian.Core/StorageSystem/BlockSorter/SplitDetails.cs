using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.BlockSorter
{
    public struct SplitDetails
    {
        public bool IsSplit;
        public uint LesserIndex;
        public long Key;
        public uint GreaterIndex;
    }
}
