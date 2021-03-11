//******************************************************************************************************
//  PageListTest.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  2/1/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System.Collections.Generic;
using GSF.IO.Unmanaged;
using NUnit.Framework;
using System;

namespace GSF.IO.FileStructure.Media.Test
{
    /// <summary>
    ///This is a test class for PageMetaDataListTest and is intended
    ///to contain all PageMetaDataListTest Unit Tests
    ///</summary>
    [TestFixture()]
    public class PageListTest
    {

        /// <summary>
        ///A test for PageMetaDataList Constructor
        ///</summary>
        [Test()]
        unsafe public void PageMetaDataListConstructorTest()
        {
            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);

            using (PageList target = new PageList(Globals.MemoryPool))
            {
                target.Dispose();
            }
            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
            using (PageList target2 = new PageList(Globals.MemoryPool))
            {
                target2.AllocateNewPage(1);
                Assert.AreNotEqual(0, Globals.MemoryPool.AllocatedBytes);
            }

            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
        }

        /// <summary>
        ///A test for AllocateNewPage
        ///</summary>
        [Test()]
        public void AllocateNewPageTest()
        {
            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
            using (PageList target = new PageList(Globals.MemoryPool))
            {
                Assert.AreEqual(0, target.AllocateNewPage(0));
                Assert.AreEqual(Globals.MemoryPool.PageSize * 1, Globals.MemoryPool.AllocatedBytes);
                Assert.AreEqual(1, target.AllocateNewPage(2));
                Assert.AreEqual(Globals.MemoryPool.PageSize * 2, Globals.MemoryPool.AllocatedBytes);
                target.DoCollection(32, new HashSet<int>(new int[] { 1 }), GetEventArgs());
                Assert.AreEqual(Globals.MemoryPool.PageSize * 1, Globals.MemoryPool.AllocatedBytes);
                Assert.AreEqual(0, target.AllocateNewPage(0));
                Assert.AreEqual(2, target.AllocateNewPage(24352));

                Assert.AreNotEqual(IntPtr.Zero, target.GetPointerToPage(0, 0));
                Assert.AreNotEqual(IntPtr.Zero, target.GetPointerToPage(1, 0));
                Assert.AreNotEqual(IntPtr.Zero, target.GetPointerToPage(2, 0));
            }
            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [Test()]
        public void DisposeTest()
        {
            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
            using (PageList target = new PageList(Globals.MemoryPool))
            {
                target.AllocateNewPage(0);
                target.Dispose();
                Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
            }
        }

        /// <summary>
        ///A test for DoCollection
        ///</summary>
        [Test()]
        public void DoCollectionTest()
        {
            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);
            using (PageList target = new PageList(Globals.MemoryPool))
            {
                target.AllocateNewPage(0);
                target.AllocateNewPage(1);
                target.AllocateNewPage(2);
                target.AllocateNewPage(3);
                target.AllocateNewPage(4);
                target.AllocateNewPage(5);
                target.AllocateNewPage(6);
                target.AllocateNewPage(7);

                target.GetPointerToPage(0, 0);
                target.GetPointerToPage(1, 1 << 0);
                target.GetPointerToPage(2, 1 << 1);
                target.GetPointerToPage(3, 1 << 1);
                target.GetPointerToPage(4, 1 << 2);
                target.GetPointerToPage(5, 1 << 3);
                target.GetPointerToPage(6, 1 << 4);
                target.GetPointerToPage(7, 1 << 6);

                Assert.AreEqual(2, target.DoCollection(1, new HashSet<int>(), GetEventArgs()));
                Assert.AreEqual(2, target.DoCollection(1, new HashSet<int>(), GetEventArgs()));
                Assert.AreEqual(1, target.DoCollection(1, new HashSet<int>(), GetEventArgs()));
                Assert.AreEqual(1, target.DoCollection(1, new HashSet<int>(), GetEventArgs()));
                Assert.AreEqual(1, target.DoCollection(1, new HashSet<int>(), GetEventArgs()));
                Assert.AreEqual(0, target.DoCollection(1, new HashSet<int>(), GetEventArgs()));
                Assert.AreEqual(1, target.DoCollection(1, new HashSet<int>(), GetEventArgs()));
            }
            Assert.AreEqual(0, Globals.MemoryPool.AllocatedBytes);

        }

        static CollectionEventArgs GetEventArgs()
        {
            return new CollectionEventArgs((x) => Globals.MemoryPool.ReleasePage(x), MemoryPoolCollectionMode.Normal, 0);
        }

    }

    static class Extension
    {
        public static int AllocateNewPage(this PageList page, int pageNumber)
        {
            Globals.MemoryPool.AllocatePage(out int index, out IntPtr ptr);
            return page.AddNewPage(pageNumber, ptr, index);
        }
    }
}

