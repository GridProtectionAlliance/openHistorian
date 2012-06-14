using openHistorian.V2.IO.Unmanaged;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.Test
{


    /// <summary>
    ///This is a test class for PageMetaDataListTest and is intended
    ///to contain all PageMetaDataListTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PageMetaDataListTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for PageMetaDataList Constructor
        ///</summary>
        [TestMethod()]
        public void PageMetaDataListConstructorTest()
        {
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);

            using (PageMetaDataList target = new PageMetaDataList())
            {
                target.Dispose();
            }
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            PageMetaDataList target2 = new PageMetaDataList();
            target2.AllocateNewPage(1);
            Assert.AreNotEqual(0, Globals.BufferPool.AllocatedBytes);

            target2 = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
        }

        /// <summary>
        ///A test for AllocateNewPage
        ///</summary>
        [TestMethod()]
        public void AllocateNewPageTest()
        {
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            using (PageMetaDataList target = new PageMetaDataList())
            {
                Assert.AreEqual(0, target.AllocateNewPage(0));
                Assert.AreEqual(Globals.BufferPool.PageSize * 1, Globals.BufferPool.AllocatedBytes);
                Assert.AreEqual(1, target.AllocateNewPage(2));
                Assert.AreEqual(Globals.BufferPool.PageSize * 2, Globals.BufferPool.AllocatedBytes);
                target.DoCollection(32, (x, y) => x == 0);
                Assert.AreEqual(Globals.BufferPool.PageSize * 1, Globals.BufferPool.AllocatedBytes);
                Assert.AreEqual(0, target.AllocateNewPage(0));
                Assert.AreEqual(2, target.AllocateNewPage(24352));
                Assert.AreEqual(0, target.GetMetaDataPage(0).PositionIndex);
                Assert.AreEqual(2, target.GetMetaDataPage(1).PositionIndex);
                Assert.AreEqual(24352, target.GetMetaDataPage(2).PositionIndex);
            }
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
        }

        /// <summary>
        ///A test for ClearDirtyBits
        ///</summary>
        [TestMethod()]
        public void ClearDirtyBitsTest()
        {
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            using (PageMetaDataList target = new PageMetaDataList())
            {
                Assert.AreEqual(0, target.AllocateNewPage(0));
                Assert.AreEqual(0x23, target.GetMetaDataPage(0, 0x23, 0).IsDirtyFlags);
                Assert.AreEqual(0x63, target.GetMetaDataPage(0, 0x40, 0).IsDirtyFlags);

                Assert.AreEqual(0, target.DoCollection(32, (x, y) => true));
                target.ClearDirtyBits(0);
                Assert.AreEqual(1, target.DoCollection(0, (x, y) => true));
            }
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            using (PageMetaDataList target = new PageMetaDataList())
            {
                target.AllocateNewPage(0);
                target.Dispose();
                Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            }
        }

        /// <summary>
        ///A test for DoCollection
        ///</summary>
        [TestMethod()]
        public void DoCollectionTest()
        {
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            using (PageMetaDataList target = new PageMetaDataList())
            {
                target.AllocateNewPage(0);
                target.AllocateNewPage(0);
                target.AllocateNewPage(0);
                target.AllocateNewPage(0);
                target.AllocateNewPage(0);
                target.AllocateNewPage(0);
                target.AllocateNewPage(0);
                target.AllocateNewPage(0);

                target.GetMetaDataPage(0, 0, 0);
                target.GetMetaDataPage(1, 0, 1 << 0);
                target.GetMetaDataPage(2, 0, 1 << 1);
                target.GetMetaDataPage(3, 0, 1 << 1);
                target.GetMetaDataPage(4, 0, 1 << 2);
                target.GetMetaDataPage(5, 0, 1 << 3);
                target.GetMetaDataPage(6, 0, 1 << 4);
                target.GetMetaDataPage(7, 0, 1 << 6);

                Assert.AreEqual(2, target.DoCollection(1, (x, y) => true));
                Assert.AreEqual(2, target.DoCollection(1, (x, y) => true));
                Assert.AreEqual(1, target.DoCollection(1, (x, y) => true));
                Assert.AreEqual(1, target.DoCollection(1, (x, y) => true));
                Assert.AreEqual(1, target.DoCollection(1, (x, y) => true));
                Assert.AreEqual(0, target.DoCollection(1, (x, y) => true));
                Assert.AreEqual(1, target.DoCollection(1, (x, y) => true));
            }
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);

        }

    }
}
