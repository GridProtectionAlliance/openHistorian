using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian.StorageSystem.BlockSorter
{
    class TimeKey : IBlockKey8
    {
        DateTime m_time;
        public TimeKey(DateTime time)
        {
            m_time = time;
        }
        public void GetKey(out long key1)
        {
            key1 = m_time.Ticks;
        }
    }
}
