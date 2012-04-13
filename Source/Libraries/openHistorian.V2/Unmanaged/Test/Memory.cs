//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.V2.Unmanaged
//{
//    static class MemoryTest
//    {
//        public static void Test()
//        {
//            Memory block = Memory.Allocate(1, false);
//            if (block.Address == IntPtr.Zero)
//                throw new Exception();
//            if (block.Size != 1)
//                throw new Exception();
//            block.Release();
//            if (block.Address != IntPtr.Zero)
//                throw new Exception();
//            if (block.Size != 0)
//                throw new Exception();
//            block.Release();

//            block = Memory.Allocate(1, true);
//            if (block.Address == IntPtr.Zero)
//                throw new Exception();
//            if (block.Size != 2 * 1024 * 1024)
//                throw new Exception();
//            block.Release();
//            if (block.Address != IntPtr.Zero)
//                throw new Exception();
//            if (block.Size != 0)
//                throw new Exception();
//            block.Release();

//            var info = new Microsoft.VisualBasic.Devices.ComputerInfo();
//            GC.Collect();
//            GC.WaitForPendingFinalizers();

//            long mem = (long)info.AvailablePhysicalMemory;

//            //Allocate 1GB
//            List<Memory> blocks = new List<Memory>();
//            for (int x = 0; x < 100; x++)
//                blocks.Add(Memory.Allocate(10000000, true));
//            GC.Collect();
//            GC.WaitForPendingFinalizers();
//            long mem2 = (long)info.AvailablePhysicalMemory;

//            //Verify that it increased by more than 500MB
//            if (mem2 > mem + 10000000 * 50)
//            {
//                throw new Exception();
//            }
            
//            //Release through collection
//            blocks = null;
//            GC.Collect();
//            GC.WaitForPendingFinalizers();
//            //Verify that the difference between the start and the end is less than 500MB
//            long mem3 = (long)info.AvailablePhysicalMemory;

//            if (Math.Abs(mem3 - mem) > 10000000 * 50)
//            {
//                throw new Exception();
//            }


//        }
//    }
//}
