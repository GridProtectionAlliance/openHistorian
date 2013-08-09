//using GSF;
//using NUnit.Framework;
//using openHistorian.FileStructure.IO;
//using System;
//using GSF.UnmanagedMemory;

//namespace openHistorian.FileStructure.IO
//{
//    /// <summary>
//    ///This is a test class for PageMetaDataListTest and is intended
//    ///to contain all PageMetaDataListTest Unit Tests
//    ///</summary>
//    [TestFixture()]
//    public class PageListTest
//    {

//        /// <summary>
//        ///A test for PageMetaDataList Constructor
//        ///</summary>
//        [Test()]
//        unsafe public void PageMetaDataListConstructorTest()
//        {
//            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);

//            using (PageList target = new PageList(Globals.BufferPool))
//            {
//                target.Dispose();
//            }
//            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
//            using (PageList target2 = new PageList(Globals.BufferPool))
//            {
//                target2.AllocateNewPage(1);
//                Assert.AreNotEqual(0, Globals.BufferPool.AllocatedBytes);
//            }

//            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
//        }

//        /// <summary>
//        ///A test for AllocateNewPage
//        ///</summary>
//        [Test()]
//        public void AllocateNewPageTest()
//        {
//            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
//            using (PageList target = new PageList(Globals.BufferPool))
//            {
//                Assert.AreEqual(0, target.AllocateNewPage(0));
//                Assert.AreEqual(Globals.BufferPool.PageSize * 1, Globals.BufferPool.AllocatedBytes);
//                Assert.AreEqual(1, target.AllocateNewPage(2));
//                Assert.AreEqual(Globals.BufferPool.PageSize * 2, Globals.BufferPool.AllocatedBytes);
//                target.DoCollection(32, (x) => x == 0, GetEventArgs());
//                Assert.AreEqual(Globals.BufferPool.PageSize * 1, Globals.BufferPool.AllocatedBytes);
//                Assert.AreEqual(0, target.AllocateNewPage(0));
//                Assert.AreEqual(2, target.AllocateNewPage(24352));

//                Assert.AreNotEqual(IntPtr.Zero, target.GetPointerToPage(0,0));
//                Assert.AreNotEqual(IntPtr.Zero, target.GetPointerToPage(1, 0));
//                Assert.AreNotEqual(IntPtr.Zero, target.GetPointerToPage(2, 0));
//            }
//            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
//        }

//        /// <summary>
//        ///A test for Dispose
//        ///</summary>
//        [Test()]
//        public void DisposeTest()
//        {
//            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
//            using (PageList target = new PageList(Globals.BufferPool))
//            {
//                target.AllocateNewPage(0);
//                target.Dispose();
//                Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
//            }
//        }

//        /// <summary>
//        ///A test for DoCollection
//        ///</summary>
//        [Test()]
//        public void DoCollectionTest()
//        {
//            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);
//            using (PageList target = new PageList(Globals.BufferPool))
//            {
//                target.AllocateNewPage(0);
//                target.AllocateNewPage(1);
//                target.AllocateNewPage(2);
//                target.AllocateNewPage(3);
//                target.AllocateNewPage(4);
//                target.AllocateNewPage(5);
//                target.AllocateNewPage(6);
//                target.AllocateNewPage(7);

//                target.GetPointerToPage(0, 0);
//                target.GetPointerToPage(1, 1 << 0);
//                target.GetPointerToPage(2, 1 << 1);
//                target.GetPointerToPage(3, 1 << 1);
//                target.GetPointerToPage(4, 1 << 2);
//                target.GetPointerToPage(5, 1 << 3);
//                target.GetPointerToPage(6, 1 << 4);
//                target.GetPointerToPage(7, 1 << 6);

//                Assert.AreEqual(2, target.DoCollection(1, (x) => true, GetEventArgs()));
//                Assert.AreEqual(2, target.DoCollection(1, (x) => true, GetEventArgs()));
//                Assert.AreEqual(1, target.DoCollection(1, (x) => true, GetEventArgs()));
//                Assert.AreEqual(1, target.DoCollection(1, (x) => true, GetEventArgs()));
//                Assert.AreEqual(1, target.DoCollection(1, (x) => true, GetEventArgs()));
//                Assert.AreEqual(0, target.DoCollection(1, (x) => true, GetEventArgs()));
//                Assert.AreEqual(1, target.DoCollection(1, (x) => true, GetEventArgs()));
//            }
//            Assert.AreEqual(0, Globals.BufferPool.AllocatedBytes);

//        }

//        static CollectionEventArgs GetEventArgs()
//        {
//            return new CollectionEventArgs((x) =>
//            {
//                Globals.BufferPool.ReleasePage(x);
//                return true;
//            }
//                                           , BufferPoolCollectionMode.Normal, 0);
//        }

//    }

//    static class Extension
//    {
//        public static int AllocateNewPage(this PageList page, int pageNumber)
//        {
//            int index;
//            IntPtr ptr;
//            Globals.BufferPool.AllocatePage(out index, out ptr);
//            return page.AllocateNewPage(pageNumber, ptr, index);
//        }
//    }
//}

