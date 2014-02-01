using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GSF.IO.Unmanaged
{
    public static class MemoryPoolTest
    {
        public static void TestMemoryLeak()
        {
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
        }
    }
}
