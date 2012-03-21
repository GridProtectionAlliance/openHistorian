using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.Unmanaged
{
    static class BufferPoolTest
    {
        public static void Test()
        {
            BufferPool.RequestCollection += new Action<BufferPoolCollectionMode>(BufferPool_RequestCollection);
            long memory = BufferPool.SystemTotalPhysicalMemory;
            if (!BufferPool.IsUsingLargePageSizes)
                throw new Exception();
            long minMem = BufferPool.MinimumMemoryUsage;
            long maxMemory = BufferPool.MaximumMemoryUsage;

            if (memory == 1 || minMem == 1 || maxMemory == 1)
                memory = memory;



        }

        static void BufferPool_RequestCollection(BufferPoolCollectionMode obj)
        {
            throw new NotImplementedException();
        }

        
    }
}
