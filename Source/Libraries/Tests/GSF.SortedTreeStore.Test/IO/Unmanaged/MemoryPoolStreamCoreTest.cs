//******************************************************************************************************
//  MemoryPoolStreamCoreTest.cs - Gbtc
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
//  2/10/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using NUnit.Framework;

namespace GSF.IO.Unmanaged.Test
{
    [TestFixture]
    class MemoryPoolStreamCoreTest
    {
        [Test()]
        public void TestAllocateAndDeallocate()
        {
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
            using (MemoryPoolStreamCore ms = new MemoryPoolStreamCore())
            {
                BlockArguments args = new BlockArguments();
                ms.GetBlock(args);
                Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, Globals.MemoryPool.PageSize);
                ms.GetBlock(args);
                Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, Globals.MemoryPool.PageSize);
                args.Position = Globals.MemoryPool.PageSize;
                ms.GetBlock(args);
                Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 2 * Globals.MemoryPool.PageSize);
            }
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
        }

        [Test()]
        public void TestConstructor()
        {
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
            using (MemoryPoolStreamCore ms = new MemoryPoolStreamCore())
            {
                Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
            }
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
        }

        [Test()]
        public void TestAlignment()
        {
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
            using (MemoryPoolStreamCore ms = new MemoryPoolStreamCore())
            {
                BlockArguments args = new BlockArguments();
                ms.ConfigureAlignment(41211, 4096);
                Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
                args.Position = 41211;
                ms.GetBlock(args);
                Assert.AreEqual(41211L, args.FirstPosition);
                Assert.AreEqual(Globals.MemoryPool.AllocatedBytes - 41211 % 4096, args.Length);
                Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, Globals.MemoryPool.PageSize);
            }
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
        }

    }
}

