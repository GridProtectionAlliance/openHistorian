using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian.FileStructure.TimeSeriesData
{

    struct IndexNodes
    {
        public long NodeValue;
        public uint NodePage;
        public uint Offset;
        public NodeType NodeType;
    }
    enum NodeType : byte
    {
        UnUsed=0,
        RootNode = 1,
        InternalNode = 2,
        LeafNode = 3
    }
}
