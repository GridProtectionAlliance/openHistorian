using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.Generic
{
    public partial class BPlusTree<TKey, TValue>
    {
        public struct SplitDetails
        {
            public bool IsSplit;
            public uint LesserIndex;
            public TKey Key;
            public uint GreaterIndex;
        }
    }
}
