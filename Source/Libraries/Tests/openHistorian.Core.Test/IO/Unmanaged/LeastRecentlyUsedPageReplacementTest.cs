//using System;
//using GSF.UnmanagedMemory;
//using NUnit.Framework;

//namespace GSF.IO.Unmanaged.Test
//{
//    /// <summary>
//    ///This is a test class for LeastRecentlyUsedPageReplacementTest and is intended
//    ///to contain all LeastRecentlyUsedPageReplacementTest Unit Tests
//    ///</summary>
//    [TestFixture()]
//    public class LeastRecentlyUsedPageReplacementTest
//    {
//        /// <summary>
//        ///A test for LeastRecentlyUsedPageReplacement Constructor
//        ///</summary>
//        [Test()]
//        public void LeastRecentlyUsedPageReplacementConstructorTest()
//        {
//            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);

//            using (LeastRecentlyUsedPageReplacement target = new LeastRecentlyUsedPageReplacement(4096, Globals.MemoryPool))
//            {
//                Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//                using (LeastRecentlyUsedPageReplacement.IoSession io = target.CreateNewIoSession())
//                {
//                    Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//                    io.TryAddNewPage(0, new byte[Globals.MemoryPool.PageSize], 0, Globals.MemoryPool.PageSize);
//                    Assert.AreNotEqual(0, Globals.MemoryPool.AllocatedBytes);
//                }
//                target.Dispose();
//            }
//            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);

//            using (LeastRecentlyUsedPageReplacement target2 = new LeastRecentlyUsedPageReplacement(4096, Globals.MemoryPool))
//            using (LeastRecentlyUsedPageReplacement.IoSession io2 = target2.CreateNewIoSession())
//            {
//                io2.TryAddNewPage(0, new byte[Globals.MemoryPool.PageSize], 0, Globals.MemoryPool.PageSize);
//                Assert.AreNotEqual(0, Globals.MemoryPool.AllocatedBytes);
//            }
//            GC.Collect();
//            GC.WaitForPendingFinalizers();

//            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//        }

//        /// <summary>
//        ///A test for ClearDirtyBits
//        ///</summary>
//        [Test()]
//        public void ClearDirtyBitsTest()
//        {
//            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//            using (LeastRecentlyUsedPageReplacement target = new LeastRecentlyUsedPageReplacement(4096, Globals.MemoryPool))
//            {
//                Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//                using (LeastRecentlyUsedPageReplacement.IoSession io = target.CreateNewIoSession())
//                {
//                    Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//                    io.TryAddNewPage(0, new byte[Globals.MemoryPool.PageSize], 0, Globals.MemoryPool.PageSize);
//                    LeastRecentlyUsedPageReplacement.SubPageMetaData metaData;
//                    io.TryGetSubPage(0, true, out metaData);
//                    foreach (PageMetaDataList.PageMetaData page in target.GetDirtyPages(true))
//                    {
//                        Assert.Fail();
//                    }
//                    io.Clear();
//                    foreach (PageMetaDataList.PageMetaData page in target.GetDirtyPages(true))
//                    {
//                        target.ClearDirtyBits(page);
//                    }
//                    foreach (PageMetaDataList.PageMetaData page in target.GetDirtyPages())
//                    {
//                        Assert.Fail();
//                    }
//                    Assert.AreNotEqual(0, Globals.MemoryPool.AllocatedBytes);
//                }

//                target.Dispose();
//            }
//            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//        }

//        /// <summary>
//        ///A test for CreateNewIoSession
//        ///</summary>
//        [Test()]
//        public void CreateNewIoSessionTest()
//        {
//            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//            using (LeastRecentlyUsedPageReplacement target = new LeastRecentlyUsedPageReplacement(4096, Globals.MemoryPool))
//            {
//                Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//                LeastRecentlyUsedPageReplacement.IoSession io1 = target.CreateNewIoSession();

//                io1.TryAddNewPage(0, new byte[Globals.MemoryPool.PageSize], 0, Globals.MemoryPool.PageSize);
//                LeastRecentlyUsedPageReplacement.SubPageMetaData metaData1;
//                io1.TryGetSubPage(0, false, out metaData1);

//                LeastRecentlyUsedPageReplacement.IoSession io2 = target.CreateNewIoSession();

//                LeastRecentlyUsedPageReplacement.SubPageMetaData metaData2;
//                Assert.AreEqual(true, io2.TryGetSubPage(200, true, out metaData2));

//                LeastRecentlyUsedPageReplacement.SubPageMetaData metaData3;
//                LeastRecentlyUsedPageReplacement.IoSession io3 = target.CreateNewIoSession();
//                Assert.AreEqual(true, io3.TryGetSubPage(4099, true, out metaData3));

//                LeastRecentlyUsedPageReplacement.IoSession io4 = target.CreateNewIoSession();
//                io4.TryAddNewPage(65536, new byte[Globals.MemoryPool.PageSize], 0, Globals.MemoryPool.PageSize);
//                LeastRecentlyUsedPageReplacement.SubPageMetaData metaData4;
//                io4.TryGetSubPage(65536, true, out metaData4);

//                Assert.AreEqual(false, metaData1.IsDirty);
//                Assert.AreEqual(0, metaData1.Position);

//                Assert.AreEqual(true, metaData2.IsDirty);
//                Assert.AreEqual(0, metaData2.Position);
//                Assert.AreEqual(true, io1.TryGetSubPage(0, false, out metaData1));

//                Assert.AreEqual(true, metaData1.IsDirty);
//                Assert.AreEqual(0, metaData1.Position);

//                Assert.AreEqual(true, metaData3.IsDirty);
//                Assert.AreEqual(4096, metaData3.Position);

//                Assert.AreEqual(true, metaData4.IsDirty);
//                Assert.AreEqual(65536, metaData4.Position);

//                foreach (PageMetaDataList.PageMetaData page in target.GetDirtyPages(true))
//                {
//                    Assert.Fail();
//                }

//                io1.Clear();
//                io2.Dispose();
//                io2 = null;
//                io3.Clear();
//                GC.Collect();
//                GC.WaitForPendingFinalizers();

//                foreach (PageMetaDataList.PageMetaData page in target.GetDirtyPages(true))
//                {
//                    target.ClearDirtyBits(page);
//                }

//                Assert.AreEqual(0, target.DoCollection(GetEventArgs()));

//                Assert.AreEqual(0, target.DoCollection(GetEventArgs()));
//                Assert.AreEqual(1, target.DoCollection(GetEventArgs())); //There have been 4 calls

//                io4.Clear();

//                Assert.AreEqual(0, target.DoCollection(GetEventArgs()));
//                foreach (PageMetaDataList.PageMetaData page in target.GetDirtyPages(true))
//                {
//                    target.ClearDirtyBits(page);
//                }
//                Assert.AreEqual(1, target.DoCollection(GetEventArgs())); //There have been 4 calls


//                target.Dispose();
//            }
//            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
//        }

//        private static CollectionEventArgs GetEventArgs()
//        {
//            return new CollectionEventArgs((x) =>
//            {
//                Globals.MemoryPool.ReleasePage(x);
//                return true;
//            }
//                                           , BufferPoolCollectionMode.Normal, 0);
//        }
//    }
//}