using System;
using System.Collections.Generic;
using GSF;
using NUnit.Framework;
using GSF.UnmanagedMemory;

namespace GSF.IO.Unmanaged.Test
{

    /// <summary>
    ///This is a test class for LeastRecentlyUsedPageReplacementTest and is intended
    ///to contain all LeastRecentlyUsedPageReplacementTest Unit Tests
    ///</summary>
    [TestFixture()]
    public class LeastRecentlyUsedPageReplacementTest
    {

        /// <summary>
        ///A test for LeastRecentlyUsedPageReplacement Constructor
        ///</summary>
        [Test()]
        public void LeastRecentlyUsedPageReplacementConstructorTest()
        {

            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);

            using (LeastRecentlyUsedPageReplacement target = new LeastRecentlyUsedPageReplacement(4096, Globals.BufferPool))
            {
                Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
                using (var io = target.CreateNewIoSession())
                {
                    Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
                    io.TryAddNewPage(0, new byte[Globals.BufferPool.PageSize], 0, Globals.BufferPool.PageSize);
                    Assert.AreNotEqual(0, Globals.BufferPool.AllocatedBytes);
                }
                target.Dispose();
            }
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);

            using (var target2 = new LeastRecentlyUsedPageReplacement(4096, Globals.BufferPool))
            using (var io2 = target2.CreateNewIoSession())
            {
                io2.TryAddNewPage(0, new byte[Globals.BufferPool.PageSize], 0, Globals.BufferPool.PageSize);
                Assert.AreNotEqual(0, Globals.BufferPool.AllocatedBytes);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
        }

        /// <summary>
        ///A test for ClearDirtyBits
        ///</summary>
        [Test()]
        public void ClearDirtyBitsTest()
        {
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            using (LeastRecentlyUsedPageReplacement target = new LeastRecentlyUsedPageReplacement(4096, Globals.BufferPool))
            {
                Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
                using (var io = target.CreateNewIoSession())
                {
                    Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
                    io.TryAddNewPage(0, new byte[Globals.BufferPool.PageSize], 0, Globals.BufferPool.PageSize);
                    LeastRecentlyUsedPageReplacement.SubPageMetaData metaData;
                    io.TryGetSubPage(0, true, out metaData);
                    foreach (var page in target.GetDirtyPages(true))
                    {
                        Assert.Fail();
                    }
                    io.Clear();
                    foreach (var page in target.GetDirtyPages(true))
                    {
                        target.ClearDirtyBits(page);
                    }
                    foreach (var page in target.GetDirtyPages())
                    {
                        Assert.Fail();
                    }
                    Assert.AreNotEqual(0, Globals.BufferPool.AllocatedBytes);
                }

                target.Dispose();
            }
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
        }

        /// <summary>
        ///A test for CreateNewIoSession
        ///</summary>
        [Test()]
        public void CreateNewIoSessionTest()
        {
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            using (LeastRecentlyUsedPageReplacement target = new LeastRecentlyUsedPageReplacement(4096, Globals.BufferPool))
            {
                Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
                var io1 = target.CreateNewIoSession();

                io1.TryAddNewPage(0, new byte[Globals.BufferPool.PageSize], 0, Globals.BufferPool.PageSize);
                LeastRecentlyUsedPageReplacement.SubPageMetaData metaData1;
                io1.TryGetSubPage(0, false, out metaData1);

                var io2 = target.CreateNewIoSession();

                LeastRecentlyUsedPageReplacement.SubPageMetaData metaData2;
                Assert.AreEqual(true, io2.TryGetSubPage(200, true, out metaData2));

                LeastRecentlyUsedPageReplacement.SubPageMetaData metaData3;
                var io3 = target.CreateNewIoSession();
                Assert.AreEqual(true, io3.TryGetSubPage(4099, true, out metaData3));

                var io4 = target.CreateNewIoSession();
                io4.TryAddNewPage(65536, new byte[Globals.BufferPool.PageSize], 0, Globals.BufferPool.PageSize);
                LeastRecentlyUsedPageReplacement.SubPageMetaData metaData4;
                io4.TryGetSubPage(65536, true, out metaData4);

                Assert.AreEqual(false, metaData1.IsDirty);
                Assert.AreEqual(0, metaData1.Position);

                Assert.AreEqual(true, metaData2.IsDirty);
                Assert.AreEqual(0, metaData2.Position);
                Assert.AreEqual(true, io1.TryGetSubPage(0, false, out metaData1));

                Assert.AreEqual(true, metaData1.IsDirty);
                Assert.AreEqual(0, metaData1.Position);

                Assert.AreEqual(true, metaData3.IsDirty);
                Assert.AreEqual(4096, metaData3.Position);

                Assert.AreEqual(true, metaData4.IsDirty);
                Assert.AreEqual(65536, metaData4.Position);

                foreach (var page in target.GetDirtyPages(true))
                {
                    Assert.Fail();
                }

                io1.Clear();
                io2.Dispose();
                io2 = null;
                io3.Clear();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                foreach (var page in target.GetDirtyPages(true))
                {
                    target.ClearDirtyBits(page);
                }

                Assert.AreEqual(0, target.DoCollection(GetEventArgs()));

                Assert.AreEqual(0, target.DoCollection(GetEventArgs()));
                Assert.AreEqual(1, target.DoCollection(GetEventArgs())); //There have been 4 calls

                io4.Clear();

                Assert.AreEqual(0, target.DoCollection(GetEventArgs()));
                foreach (var page in target.GetDirtyPages(true))
                {
                    target.ClearDirtyBits(page);
                }
                Assert.AreEqual(1, target.DoCollection(GetEventArgs())); //There have been 4 calls


                target.Dispose();
            }
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
        }
        static CollectionEventArgs GetEventArgs()
        {
            return new CollectionEventArgs((x) =>
                {
                    Globals.BufferPool.ReleasePage(x);
                    return true;
                }
                                           , BufferPoolCollectionMode.Normal, 0);
        }

    }
}
