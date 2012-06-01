using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace openHistorian.V2.UnmanagedMemory
{
    [TestClass()]
    public class MemoryTest
    {
        [TestMethod()]
        public void Test()
        {
            Assert.AreEqual(BufferPool.AllocatedBytes, 0L);

            Memory block = Memory.Allocate(1, false);
            if (block.Address == IntPtr.Zero)
                throw new Exception();
            if (block.Size != 1)
                throw new Exception();
            block.Release();
            if (block.Address != IntPtr.Zero)
                throw new Exception();
            if (block.Size != 0)
                throw new Exception();
            block.Release();

            if (WinApi.CanAllocateLargePage)
            {
                block = Memory.Allocate(1, true);
                if (block.Address == IntPtr.Zero)
                    throw new Exception();
                if (block.Size != 2 * 1024 * 1024)
                    throw new Exception();
                block.Release();
                if (block.Address != IntPtr.Zero)
                    throw new Exception();
                if (block.Size != 0)
                    throw new Exception();
                block.Release();
            }
            else
            {
                //Assert.Inconclusive("Could not test large pages");
            }

            var info = new Microsoft.VisualBasic.Devices.ComputerInfo();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            long mem = (long)info.AvailablePhysicalMemory;

            //Allocate 100MB
            List<Memory> blocks = new List<Memory>();
            for (int x = 0; x < 10; x++)
                blocks.Add(Memory.Allocate(10000000, true));
            GC.Collect();
            GC.WaitForPendingFinalizers();
            long mem2 = (long)info.AvailablePhysicalMemory;

            //Verify that it increased by more than 50MB
            if (mem2 > mem + 1000000 * 50)
            {
                throw new Exception();
            }

            //Release through collection
            blocks = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //Verify that the difference between the start and the end is less than 50MB
            long mem3 = (long)info.AvailablePhysicalMemory;

            if (Math.Abs(mem3 - mem) > 1000000 * 50)
            {
                throw new Exception();
            }

            Assert.AreEqual(BufferPool.AllocatedBytes, 0L);
        }
    }
}
