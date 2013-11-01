//using System;
//using NUnit.Framework;

//namespace GSF.IO.Unmanaged.Test
//{
//    [TestFixture]
//    class MemoryStreamCoreTest
//    {
//        [Test()]
//        public void TestAllocateAndDeallocate()
//        {
//            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
//            using (MemoryStreamCore ms = new MemoryStreamCore())
//            {
//                IntPtr ptr;
//                long firstPos;
//                int length;
//                ms.GetBlock(0, out ptr, out firstPos, out length);
//                Assert.AreEqual(Globals.BufferPool.AllocatedBytes, Globals.BufferPool.PageSize);
//                ms.GetBlock(0, out ptr, out firstPos, out length);
//                Assert.AreEqual(Globals.BufferPool.AllocatedBytes, Globals.BufferPool.PageSize);
//                ms.GetBlock(Globals.BufferPool.PageSize, out ptr, out firstPos, out length);
//                Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 2 * Globals.BufferPool.PageSize);
//            }
//            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
//        }

//        [Test()]
//        public void TestConstructor()
//        {
//            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
//            using (MemoryStreamCore ms = new MemoryStreamCore())
//            {
//                Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
//            }
//            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
//        }

//        [Test()]
//        public void TestAlignment()
//        {
//            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
//            using (MemoryStreamCore ms = new MemoryStreamCore())
//            {
//                ms.ConfigureAlignment(41211, 4096);
//                Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
//                IntPtr ptr;
//                long firstPos;
//                int length;
//                ms.GetBlock(41211, out ptr, out firstPos, out length);
//                Assert.AreEqual(41211L, firstPos);
//                Assert.AreEqual(Globals.BufferPool.AllocatedBytes - (41211 % 4096), length);
//                Assert.AreEqual(Globals.BufferPool.AllocatedBytes, Globals.BufferPool.PageSize);
//            }
//            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
//        }

//    }
//}

