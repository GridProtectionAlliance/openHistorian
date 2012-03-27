//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.Core.StorageSystem.BufferPool
//{
//    public unsafe class PoolList
//    {
//        int m_bufferSize;
//        byte[] m_array;
//        int m_size;
//        long CollectionCount;

//        Dictionary<long, MemoryUnit> m_activeAllocations;
//        Dictionary<long, int> m_allAllocations;
//        Dictionary<uint, IDataSetIo> m_dataIo;
//        Queue<int> m_freePages;
//        Queue<int> m_usedPages;
//        Queue<int> m_dirtyPages; 

//        public PoolList(int bufferSize)
//        {
//            m_size = bufferSize;
//            m_array = new byte[16 * bufferSize];
//            m_activeAllocations = new Dictionary<long, MemoryUnit>(1000);
//            m_allAllocations = new Dictionary<long, int>(bufferSize * 5);
//            m_dataIo = new Dictionary<uint, IDataSetIo>(100);
            
//            m_freePages = new Queue<int>(bufferSize);
//            m_usedPages = new Queue<int>(bufferSize);
//            m_dirtyPages = new Queue<int>(bufferSize);

//            for (int x = 0; x<bufferSize; x++)
//            {
//                m_freePages.Enqueue(x);
//            }
//        }

//        MemoryUnit GetBuffer(uint fileId, uint pageId)
//        {
//            long key = (long)((ulong)fileId << 32) | pageId;
//            MemoryUnit unit;
//            if (m_activeAllocations.TryGetValue(key, out unit))
//            {
//                return unit;
//            }
//            int index;
//            if (m_allAllocations.TryGetValue(key, out index))
//            {
//                unit = GetBuffer(index);
//                m_activeAllocations.Add(key, unit);
//                return unit;
//            }
//            IDataSetIo dataIo = m_dataIo[fileId];
//            unit = GetFreeBlock();
//            dataIo.ReadBlock(pageId, unit.Pointer, m_bufferSize);
//            m_activeAllocations.Add(key,unit);
//            m_allAllocations.Add(key,unit.BufferIndex);
//            return unit;
//        }

//        MemoryUnit GetFreeBlock()
//        {
//            return null;
//        }

//        MemoryUnit GetBuffer(int index)
//        {
//            if (index < 0 || index >= m_size)
//                throw new ArgumentException("Index out of range", "index");

//            MemoryUnit buffer = new MemoryUnit();
//            buffer.BufferIndex = index;

//            index = index * 16;
//            fixed (byte* lp = m_array)
//            {
//                buffer.Pointer = (byte*)(*(long*)(lp + index));
//                buffer.FileId = *(uint*)(lp + index + 8);
//                buffer.AddressBlock = *(uint*)(lp + index + 12);
//            }
//            return buffer;
//        }

//    }
//}
