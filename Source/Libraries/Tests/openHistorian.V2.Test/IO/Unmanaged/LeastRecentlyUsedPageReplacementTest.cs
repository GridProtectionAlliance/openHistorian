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


        ///// <summary>
        /////A test for LeastRecentlyUsedPageReplacement Constructor
        /////</summary>
        //[TestMethod()]
        //public void LeastRecentlyUsedPageReplacementConstructorTest()
        //{
        //    using (LeastRecentlyUsedPageReplacement target = new LeastRecentlyUsedPageReplacement())
        //    {
        //    }
        //}

        ///// <summary>
        /////A test for AllocateNewPageMetaDataIndex
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("openHistorian.V2.dll")]
        //public void AllocateNewPageMetaDataIndexTest()
        //{
        //    using (LeastRecentlyUsedPageReplacement_Accessor target = new LeastRecentlyUsedPageReplacement_Accessor())
        //    {
        //        Assert.AreEqual(0, BufferPool.AllocatedBytes);
        //        for (int x = 0; x < 10; x++)
        //        {
        //            int index = target.AllocateNewPageMetaDataIndex(1);
        //            Assert.AreEqual(x, index);
        //            Assert.AreEqual(true, target.m_isPageMetaDataNotNull.GetBit(index));
        //            Assert.AreEqual(index + 1, target.m_isPageMetaDataNotNull.FindClearedBit());
        //        }
        //        Assert.AreNotEqual(0, BufferPool.AllocatedBytes);

        //    }
        //    Assert.AreEqual(0, BufferPool.AllocatedBytes);
        //}

        ///// <summary>
        /////A test for CreateNewIoSession
        /////</summary>
        //[TestMethod()]
        //public void CreateNewIoSessionTest()
        //{
        //    using (LeastRecentlyUsedPageReplacement target = new LeastRecentlyUsedPageReplacement())
        //    {
        //        for (int x = 0; x < 100; x++)
        //        {
        //            Assert.AreEqual(x, target.CreateNewIoSession());
        //        }
        //        for (int x = 1; x < 100; x += 2)
        //        {
        //            target.ReleaseIoSession(x);
        //        }
        //        for (int x = 1; x < 100; x += 2)
        //        {
        //            Assert.AreEqual(x, target.CreateNewIoSession());
        //        }
        //    }
        //}

        ///// <summary>
        /////A test for DoCollection
        /////</summary>
        //[TestMethod()]
        //unsafe public void DoCollectionTest()
        //{
        //    long bufferPoolBytes;
        //    Assert.AreEqual(0, BufferPool.AllocatedBytes);
        //    using (LeastRecentlyUsedPageReplacement target = new LeastRecentlyUsedPageReplacement())
        //    {
        //        int ioSession = target.CreateNewIoSession();
        //        bool isWriting = false;
        //        bool isEmptyPage;

        //        for (long position = 0; position < 1000000; position += 1024)
        //        {
        //            var pageMetaData = target.TryGetPageOrCreateNew(position, ioSession, false, out isEmptyPage);
        //            Assert.AreEqual((position & BufferPool.PageMask) == 0, isEmptyPage);
        //            Assert.AreEqual(position & ~(long)BufferPool.PageMask, pageMetaData.PagePosition);
        //            Assert.AreEqual(position & ~4095L, pageMetaData.SubPagePosition);
        //        }
        //        bufferPoolBytes = BufferPool.AllocatedBytes;
        //        target.ReleaseIoSession(ioSession);

        //        for (int x = 0; x < 32; x++) //Collect All Pages
        //            target.DoCollection();

        //        Assert.AreEqual(0, BufferPool.AllocatedBytes);
                
        //        ioSession = target.CreateNewIoSession();

        //        for (int count = 0; count < 10; count++)
        //        {
        //            for (int x = 0; x < (1 << count); x++)
        //            {
        //                var pageMetaData = target.TryGetPageOrCreateNew(count * 65536, ioSession, false, out isEmptyPage);
        //                Assert.AreEqual(x==0,isEmptyPage);
        //            }
        //        }
        //        target.ReleaseIoSession(ioSession);
        //        bufferPoolBytes = BufferPool.AllocatedBytes;

        //        while (BufferPool.AllocatedBytes > 0)
        //        {
        //            target.DoCollection();
        //            Assert.AreEqual(bufferPoolBytes - 65536, BufferPool.AllocatedBytes);
        //            bufferPoolBytes = BufferPool.AllocatedBytes;
        //        }
        //    }
        //}

        ///// <summary>
        /////A test for TryGetPageOrCreateNew
        /////</summary>
        //[TestMethod()]
        //unsafe public void TryGetPageOrCreateNewTest()
        //{
        //    long bufferPoolBytes;
        //    Assert.AreEqual(0, BufferPool.AllocatedBytes);

        //    using (LeastRecentlyUsedPageReplacement target = new LeastRecentlyUsedPageReplacement())
        //    {
        //        int ioSession = target.CreateNewIoSession();
        //        bool isWriting = false;
        //        bool isEmptyPage;

        //        for (long position = 0; position < 100000; position += 8192)
        //        {
        //            int subPageIndex = (int)((position & BufferPool.PageMask) >> LeastRecentlyUsedPageReplacement_Accessor.SubPageShiftBits);

        //            var pageMetaData = target.TryGetPageOrCreateNew(position, ioSession, isWriting, out isEmptyPage);

        //            Assert.AreEqual((position & BufferPool.PageMask) == 0, isEmptyPage);
        //            Assert.AreEqual(GenerateDirtyFlags(subPageIndex, isWriting), pageMetaData.IsDirtyFlags);
        //            Assert.AreEqual(isWriting, pageMetaData.IsSubPageDirty);
        //            Assert.AreEqual(true, pageMetaData.SubPageLocation == pageMetaData.PageLocation + (position & BufferPool.PageMask));
        //            Assert.AreEqual(position & ~(long)BufferPool.PageMask, pageMetaData.PagePosition);
        //            Assert.AreEqual(position, pageMetaData.SubPagePosition);
        //            *(long*)pageMetaData.SubPageLocation = position;
        //            isWriting = !isWriting;
        //        }

        //        bufferPoolBytes = BufferPool.AllocatedBytes;

        //        isWriting = false;
        //        for (long position = 0; position < 100000; position += 8192)
        //        {
        //            int subPageIndex = (int)((position & BufferPool.PageMask) >> LeastRecentlyUsedPageReplacement_Accessor.SubPageShiftBits);

        //            var pageMetaData = target.TryGetPageOrCreateNew(position, ioSession, false, out isEmptyPage);

        //            Assert.AreEqual(false, isEmptyPage);
        //            Assert.AreEqual(isWriting, ((pageMetaData.IsDirtyFlags >> subPageIndex) & 1) == 1);
        //            Assert.AreEqual(isWriting, pageMetaData.IsSubPageDirty);
        //            Assert.AreEqual(true, pageMetaData.SubPageLocation == pageMetaData.PageLocation + (position & BufferPool.PageMask));
        //            Assert.AreEqual(position & ~(long)BufferPool.PageMask, pageMetaData.PagePosition);
        //            Assert.AreEqual(position, pageMetaData.SubPagePosition);
        //            Assert.AreEqual(*(long*)pageMetaData.SubPageLocation, position);
        //            isWriting = !isWriting;
        //        }

        //        for (int x = 0; x < 32; x++) //Since each page is dirty, no pages should be collected.
        //            target.DoCollection();

        //        Assert.AreEqual(bufferPoolBytes, BufferPool.AllocatedBytes);

        //        //Free one block at a time until only one block is remaining.
        //        while (BufferPool.AllocatedBytes > 65536)
        //        {
        //            foreach (var dirtyPage in target.GetDirtyPages())
        //            {
        //                target.ClearDirtyBits(dirtyPage.PagePosition);
        //                break;
        //            }
        //            target.DoCollection();
        //            Assert.AreEqual(bufferPoolBytes - 65536, BufferPool.AllocatedBytes);
        //            bufferPoolBytes = BufferPool.AllocatedBytes;
        //        }

        //        //Removing the last block should do nothing since an IO Session prevents it from being removed.
        //        foreach (var dirtyPage in target.GetDirtyPages())
        //        {
        //            target.ClearDirtyBits(dirtyPage.PagePosition);
        //            break;
        //        }
        //        target.DoCollection();
        //        Assert.AreEqual(bufferPoolBytes, BufferPool.AllocatedBytes);
        //        bufferPoolBytes = BufferPool.AllocatedBytes;

        //        target.ReleaseIoSession(ioSession);
        //        foreach (var dirtyPage in target.GetDirtyPages())
        //        {
        //            target.ClearDirtyBits(dirtyPage.PagePosition);
        //            break;
        //        }
        //        target.DoCollection();
        //        Assert.AreEqual(0, BufferPool.AllocatedBytes);
        //    }
        //}
        //ushort GenerateDirtyFlags(int subPageIndex, bool isWriting)
        //{
        //    if (!isWriting)
        //        subPageIndex -= 2;
        //    ushort value = 0;
        //    for (int x = subPageIndex; x >= 0; x -= 4)
        //    {
        //        value |= (ushort)(1 << x);
        //    }
        //    return value;

        //}

    }
}
