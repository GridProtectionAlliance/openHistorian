using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.BufferPool
{
    public class DataSet
    {
        List<MemoryUnit> m_list;
        Dictionary<uint, MemoryUnit> m_cache;
        internal DataSet()
        {
            m_cache = new Dictionary<uint, MemoryUnit>();
            m_list = new List<MemoryUnit>();
        }
        internal void GarbageCollectionComplete()
        {
            int curFreePos = 0;
            for (int x = 0; x < m_list.Count; x++)
            {
                if (m_list[x].ReferencedCount != 0)
                {
                    if (curFreePos != x)
                    {
                        m_list[curFreePos] = m_list[x];
                    }
                    curFreePos++;
                }
                else
                {
                    m_cache.Remove(m_list[x].BlockIndex);
                }
            }
            m_list.RemoveRange(curFreePos, m_list.Count - curFreePos);
        }

        public bool TryGetMemoryBlock(uint blockIndex, out MemoryUnit unit)
        {
            return m_cache.TryGetValue(blockIndex, out unit);
        }

        public MemoryUnit GetMemoryBlock(uint blockIndex)
        {
            MemoryUnit unit;
            if (m_cache.TryGetValue(blockIndex, out unit))
                return unit;
            unit = Pool.GetNewBlock();
            unit.BlockIndex = blockIndex;
            m_list.Add(unit);
            m_cache.Add(blockIndex,unit);
            return unit;
        }


    }
}
