using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian.PointTypes
{
    public enum PointDataTypes : byte
    {
        Null = 0,
        UInt=7,
        Float=9,
        DateTime=13,
        NestedType = 17
    }
}
