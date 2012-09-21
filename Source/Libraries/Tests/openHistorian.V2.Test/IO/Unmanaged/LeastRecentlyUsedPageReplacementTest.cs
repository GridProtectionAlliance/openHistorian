using openHistorian.V2.IO.Unmanaged;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.Test
{
    
    /// <summary>
    ///This is a test class for LeastRecentlyUsedPageReplacementTest and is intended
    ///to contain all LeastRecentlyUsedPageReplacementTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LeastRecentlyUsedPageReplacementTest
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
        ///A test for LeastRecentlyUsedPageReplacement Constructor
        ///</summary>
        [TestMethod()]
        public void LeastRecentlyUsedPageReplacementConstructorTest()
        {

            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);

            using (LeastRecentlyUsedPageReplacement target = new LeastRecentlyUsedPageReplacement())
            {
                Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
                using (var io = target.CreateNewIoSession())
                {
                    Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
                    io.TryGetSubPageOrCreateNew(0, false, (x, y) => y = y);
                    Assert.AreNotEqual(0, Globals.BufferPool.AllocatedBytes);
                }
                target.Dispose();
            }
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);

            using (var target2 = new LeastRecentlyUsedPageReplacement())
            using (var io2 = target2.CreateNewIoSession())
            {
                io2.TryGetSubPageOrCreateNew(0, false, (x, y) => y = y);
                Assert.AreNotEqual(0, Globals.BufferPool.AllocatedBytes);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
        }

        /// <summary>
        ///A test for ClearDirtyBits
        ///</summary>
        [TestMethod()]
        public void ClearDirtyBitsTest()
        {
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            using (LeastRecentlyUsedPageReplacement target = new LeastRecentlyUsedPageReplacement())
            {
                Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
                using (var io = target.CreateNewIoSession())
                {
                    Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
                    var metaData = io.TryGetSubPageOrCreateNew(0, true, (x, y) => y = y);
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
        [TestMethod()]
        public void CreateNewIoSessionTest()
        {
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
            using (LeastRecentlyUsedPageReplacement target = new LeastRecentlyUsedPageReplacement())
            {
                Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
                var io1 = target.CreateNewIoSession();
                var metaData1 = io1.TryGetSubPageOrCreateNew(0, false, (x, y) => y = y);
                var io2 = target.CreateNewIoSession();
                var metaData2 = io2.TryGetSubPageOrCreateNew(200, true, (x, y) => y = y);
                var io3 = target.CreateNewIoSession();
                var metaData3 = io3.TryGetSubPageOrCreateNew(4099, true, (x, y) => y = y);
                var io4 = target.CreateNewIoSession();
                var metaData4 = io4.TryGetSubPageOrCreateNew(65536, true, (x, y) => y = y);

                Assert.AreEqual(false, metaData1.IsDirty);
                Assert.AreEqual(0, metaData1.Position);

                Assert.AreEqual(true, metaData2.IsDirty);
                Assert.AreEqual(0, metaData2.Position);
                metaData1 = io1.TryGetSubPageOrCreateNew(0, false, (x, y) => y = y);

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

                Assert.AreEqual(0, target.DoCollection());
                Assert.AreEqual(0, target.DoCollection());
                Assert.AreEqual(1, target.DoCollection()); //There have been 4 calls

                io4.Clear();

                Assert.AreEqual(0, target.DoCollection());
                foreach (var page in target.GetDirtyPages(true))
                {
                    target.ClearDirtyBits(page);
                }
                Assert.AreEqual(1, target.DoCollection()); //There have been 4 calls


                target.Dispose();
            }
            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
        }

    }
}
