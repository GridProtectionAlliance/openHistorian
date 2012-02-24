using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian.FileStructure.TimeSeriesData
{
    class DataIndex
    {
        IndexNodes[] Nodes;
        int Size;

        DataIndex(int Size)
        {
            Nodes = new IndexNodes[Size];
            Size = 0;
        }
        IndexNodes Find(long Value)
        {
            int mid;
            int min = 0;
            int max = Size-1;
            do
            {
                mid = (min + max) >> 1;
                if (Value > Nodes[mid].NodeValue)
                    min = mid + 1;
                else
                    max = mid - 1;
            } while (!(Nodes[mid].NodeValue == Value || min > max));
            return Nodes[mid];
        }

    }
}
