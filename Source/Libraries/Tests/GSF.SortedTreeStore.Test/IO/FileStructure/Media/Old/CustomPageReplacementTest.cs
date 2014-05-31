//using System;
//using System.Collections.Generic;
//using GSF;
//using GSF.IO.Unmanaged;
//using NUnit.Framework;

//namespace GSF.IO.FileStructure.Media.Test
//{

//    /// <summary>
//    ///This is a test class for LeastRecentlyUsedPageReplacementTest and is intended
//    ///to contain all LeastRecentlyUsedPageReplacementTest Unit Tests
//    ///</summary>
//    [TestFixture()]
//    public class CustomLeastRecentlyUsedPageReplacementTest
//    {

//        /// <summary>
//        ///A test for LeastRecentlyUsedPageReplacement Constructor
//        ///</summary>
//        [Test()]
//        public void LeastRecentlyUsedPageReplacementConstructorTest()
//        {

//            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);

//            using (PageReplacementAlgorithm target = new PageReplacementAlgorithm(Globals.MemoryPool))
//            {
//                Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//                using (var io = target.GetPageLock())
//                {
//                    Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//                    io.TryAddNewPage(0);
//                    Assert.AreNotEqual(0, Globals.MemoryPool.AllocatedBytes);
//                }
//                target.Dispose();
//            }
//            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);

//            using (var target2 = new PageReplacementAlgorithm(Globals.MemoryPool))
//            using (var io2 = target2.GetPageLock())
//            {
//                io2.TryAddNewPage(0);
//                Assert.AreNotEqual(0, Globals.MemoryPool.AllocatedBytes);
//            }
//            GC.Collect();
//            GC.WaitForPendingFinalizers();

//            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//        }

//        /// <summary>
//        ///A test for CreateNewIoSession
//        ///</summary>
//        [Test()]
//        public void CreateNewIoSessionTest()
//        {
//            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//            using (PageReplacementAlgorithm target = new PageReplacementAlgorithm(Globals.MemoryPool))
//            {
//                Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//                var io1 = target.GetPageLock();

//                io1.TryAddNewPage(0);
//                IntPtr metaData1;
//                io1.TryGetSubPage(0, out metaData1);

//                var io2 = target.GetPageLock();

//                IntPtr metaData2;
//                Assert.AreEqual(true, io2.TryGetSubPage(0, out metaData2));

//                IntPtr metaData3;
//                var io3 = target.GetPageLock();
//                Assert.AreEqual(true, io3.TryGetSubPage(0, out metaData3));

//                var io4 = target.GetPageLock();
//                io4.TryAddNewPage(65536);
//                IntPtr metaData4;
//                io4.TryGetSubPage(65536, out metaData4);

//                Assert.AreEqual(true, io1.TryGetSubPage(0, out metaData1));

//                io1.Clear();
//                io2.Dispose();
//                io2 = null;
//                io3.Clear();
//                GC.Collect();
//                GC.WaitForPendingFinalizers();

//                Assert.AreEqual(0, target.DoCollection(GetEventArgs()));
//                Assert.AreEqual(0, target.DoCollection(GetEventArgs()));
//                Assert.AreEqual(1, target.DoCollection(GetEventArgs())); //There have been 4 calls

//                io4.Clear();

//                Assert.AreEqual(1, target.DoCollection(GetEventArgs())); //There have been 4 calls


//                target.Dispose();
//            }
//            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//        }
//        static CollectionEventArgs GetEventArgs()
//        {
//            return new CollectionEventArgs((x) =>
//                {
//                    Globals.MemoryPool.ReleasePage(x);
//                    return true;
//                }
//                                           , BufferPoolCollectionMode.Normal, 0);
//        }


//    }
//    static class Extension2
//    {
//        public static bool TryAddNewPage(this PageReplacementAlgorithm page, int pageNumber)
//        {
//            int index;
//            IntPtr ptr;
//            Globals.MemoryPool.AllocatePage(out index, out ptr);
//            return page.TryAddNewPage(pageNumber, ptr, index);
//        }
//    }
//}

