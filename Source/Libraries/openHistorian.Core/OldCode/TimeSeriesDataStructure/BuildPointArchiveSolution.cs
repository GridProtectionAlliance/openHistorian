using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Historian;
using Historian.PointQueue;

namespace Historian.TimeSeriesDataStructure
{
    class BuildPointArchiveSolution
    {
        public int TotalSize;
        public void CompactLiveData(LiveDataProcessor LiveData)
        {
            TotalSize = 0;
            SortedList<int, int> CompSize = new SortedList<int, int>();
            foreach (var p in LiveData.BufferQueue.Values)
            {
                int Size = CompressionMethods.Method1.Compress(p.Queue);
                int PID = p.Queue[0].PointID;
                if (CompSize.ContainsKey(PID))
                    CompSize[PID] += Size;
                else
                    CompSize.Add(PID, Size);
                TotalSize += Size;
            }
            StringBuilder SB = new StringBuilder();
            foreach (var s in CompSize)
            {
                SB.AppendLine(s.Key.ToString() + '\t' + s.Value.ToString());
            }
            System.Windows.Forms.Clipboard.SetText(SB.ToString());
        }
    }
}
