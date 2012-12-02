using NUnit.Framework;
using openHistorian.V2.IO.Unmanaged;
using System;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.IO.Unmanaged.Test
{


    /// <summary>
    ///This is a test class for PageMetaDataListTest and is intended
    ///to contain all PageMetaDataListTest Unit Tests
    ///</summary>
    [TestFixture()]
    public class PageMetaDataListTest
    {

        /// <summary>
        ///A test for PageMetaDataList Constructor
        ///</summary>
        [Test()]
        public void PageMetaDataListConstructorTest()
        {
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);

            using (PageMetaDataList target = new PageMetaDataList(Globals.BufferPool))
            {
                target.Dispose();
            }
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            using (PageMetaDataList target2 = new PageMetaDataList(Globals.BufferPool))
            {
                target2.AllocateNewPage(1);
                Assert.AreNotEqual(0, Globals.BufferPool.AllocatedBytes);
            }

            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
        }

        /// <summary>
        ///A test for AllocateNewPage
        ///</summary>
        [Test()]
        public void AllocateNewPageTest()
        {
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            using (PageMetaDataList target = new PageMetaDataList(Globals.BufferPool))
            {
                Assert.AreEqual(0, target.AllocateNewPage(0));
                Assert.AreEqual(Globals.BufferPool.PageSize * 1, Globals.BufferPool.AllocatedBytes);
                Assert.AreEqual(1, target.AllocateNewPage(2));
                Assert.AreEqual(Globals.BufferPool.PageSize * 2, Globals.BufferPool.AllocatedBytes);
                target.DoCollection(32, (x) => x == 0, GetEventArgs());
                Assert.AreEqual(Globals.BufferPool.PageSize * 1, Globals.BufferPool.AllocatedBytes);
                Assert.AreEqual(0, target.AllocateNewPage(0));
                Assert.AreEqual(2, target.AllocateNewPage(24352));
                Assert.AreEqual(0, target.GetMetaDataPage(0,0,0).PositionIndex);
                Assert.AreEqual(2, target.GetMetaDataPage(1,0,0).PositionIndex);
                Assert.AreEqual(24352, target.GetMetaDataPage(2,0,0).PositionIndex);
            }
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
        }

        /// <summary>
        ///A test for ClearDirtyBits
        ///</summary>
        [Test()]
        public void ClearDirtyBitsTest()
        {
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            using (PageMetaDataList target = new PageMetaDataList(Globals.BufferPool))
            {
                Assert.AreEqual(0, target.AllocateNewPage(0));
                Assert.AreEqual(0x23ul, target.GetMetaDataPage(0, 0x23, 0).IsDirtyFlags);
                Assert.AreEqual(0x63ul, target.GetMetaDataPage(0, 0x40, 0).IsDirtyFlags);

                Assert.AreEqual(0, target.DoCollection(32, (x) => true, GetEventArgs()));
                target.ClearDirtyBits(0);
                Assert.AreEqual(1, target.DoCollection(0, (x) => true, GetEventArgs()));
            }
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [Test()]
        public void DisposeTest()
        {
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            using (PageMetaDataList target = new PageMetaDataList(Globals.BufferPool))
            {
                target.AllocateNewPage(0);
                target.Dispose();
                Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            }
        }

        /// <summary>
        ///A test for DoCollection
        ///</summary>
        [Test()]
        public void DoCollectionTest()
        {
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            using (PageMetaDataList target = new PageMetaDataList(Globals.BufferPool))
            {
                target.AllocateNewPage(0);
                target.AllocateNewPage(1);
                target.AllocateNewPage(2);
                target.AllocateNewPage(3);
                target.AllocateNewPage(4);
                target.AllocateNewPage(5);
                target.AllocateNewPage(6);
                target.AllocateNewPage(7);

                target.GetMetaDataPage(0, 0, 0);
                target.GetMetaDataPage(1, 0, 1 << 0);
                target.GetMetaDataPage(2, 0, 1 << 1);
                target.GetMetaDataPage(3, 0, 1 << 1);
                target.GetMetaDataPage(4, 0, 1 << 2);
                target.GetMetaDataPage(5, 0, 1 << 3);
                target.GetMetaDataPage(6, 0, 1 << 4);
                target.GetMetaDataPage(7, 0, 1 << 6);

                Assert.AreEqual(2, target.DoCollection(1, (x) => true, GetEventArgs()));
                Assert.AreEqual(2, target.DoCollection(1, (x) => true, GetEventArgs()));
                Assert.AreEqual(1, target.DoCollection(1, (x) => true, GetEventArgs()));
                Assert.AreEqual(1, target.DoCollection(1, (x) => true, GetEventArgs()));
                Assert.AreEqual(1, target.DoCollection(1, (x) => true, GetEventArgs()));
                Assert.AreEqual(0, target.DoCollection(1, (x) => true, GetEventArgs()));
                Assert.AreEqual(1, target.DoCollection(1, (x) => true, GetEventArgs()));
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
