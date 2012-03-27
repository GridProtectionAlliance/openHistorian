//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.Core.StorageSystem.BufferPool
//{
//    public delegate void PendingCollection();

//    public static class Pool
//    {
//        static Pool()
//        {
//            AllocateMoreMemory();
//        }

//        static long s_maximumBytes = 10 * 1024 * 1024;
//        static object s_syncRoot = new object();
//        public static event PendingCollection CollectionBeginning;

//        static List<MemoryUnit[]> s_allocatedMemory = new List<MemoryUnit[]>();
//        static List<DataSet> s_dataSets = new List<DataSet>();

//        //Allocates at least 1MB of memory at a time.
//        const int MinimumAllocations = 256;
//        const int BlockSize = 4096;

//        static int s_currentBlock = 0;

//        public static long AllocatedSpace
//        {
//            get
//            {
//                return s_allocatedMemory.Count * BlockSize * MinimumAllocations;
//            }
//        }

//        internal static MemoryUnit GetNewBlock()
//        {
//            lock (s_syncRoot)
//            {
//                return GetNewBlock(0);
//            }
//        }

//        static MemoryUnit GetNewBlock(int count)
//        {
//            int lastBlock = s_allocatedMemory.Count;
//            int currentBlockPos = s_currentBlock / MinimumAllocations;
//            int currentPos = s_currentBlock % MinimumAllocations;

//            MemoryUnit[] currentBlock = s_allocatedMemory[currentBlockPos];
//            for (int x2 = currentPos; x2 < currentBlock.Length; x2++)
//            {
//                if (currentBlock[x2].ReferencedCount == 0)
//                {
//                    s_currentBlock += x2 - currentPos + 1;
//                    return currentBlock[x2];
//                }
//            }
//            s_currentBlock += MinimumAllocations - currentPos + 1;

//            for (int x1 = currentBlockPos + 1; x1 < lastBlock; x1++)
//            {
//                currentBlock = s_allocatedMemory[x1];

//                for (int x2 = currentPos; x2 < currentBlock.Length; x2++)
//                {
//                    if (currentBlock[x2].ReferencedCount == 0)
//                    {
//                        s_currentBlock += x2 + (x1 - currentBlockPos - 1) * MinimumAllocations;
//                        return currentBlock[x2];
//                    }
//                }
//            }
//            if (s_maximumBytes < AllocatedSpace || count == 1)
//            {
//                AllocateMoreMemory();
//                s_currentBlock = (s_allocatedMemory.Count - 1) * MinimumAllocations;
//                return s_allocatedMemory[s_allocatedMemory.Count - 1][0];
//            }
//            GarbageCollect();
//            return GetNewBlock(count + 1);
//        }

//        static void GarbageCollect()
//        {
//            foreach (var block in s_allocatedMemory)
//            {
//                foreach (var memoryUnit in block)
//                {
//                    memoryUnit.ReferencedCount >>= 1;
//                }
//            }
//            if (CollectionBeginning != null)
//                CollectionBeginning.Invoke();
//            s_currentBlock = 0;
//        }


//        public static DataSet CreateNewDataSet()
//        {
//            var set = new DataSet();
//            s_dataSets.Add(set);
//            return set;
//        }

//        static void AllocateMoreMemory()
//        {
//            MemoryUnit[] mem = new MemoryUnit[MinimumAllocations];
//            for (int x = 0; x < mem.Length; x++)
//            {
//                mem[x] = new MemoryUnit();
//            }
//            s_allocatedMemory.Add(mem);
//        }



//    }
//}
