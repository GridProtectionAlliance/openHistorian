//******************************************************************************************************
//  LargeArrayTest.cs - Gbtc
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
//  9/1/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using GSF.IO.Unmanaged.Test;
using NUnit.Framework;
using openHistorian;

namespace GSF.Collections.Test
{
    [TestFixture]
    public class LargeArrayTest
    {
        [Test]
        public void TestLargeArray()
        {
            MemoryPoolTest.TestMemoryLeak();
            TestArray(new LargeArray<int>());
            MemoryPoolTest.TestMemoryLeak();
        }

        //[Test]
        //public unsafe void TestLargeUnmanagedArray()
        //{
        //    Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0);
        //    using (LargeUnmanagedArray<int> array = new LargeUnmanagedArray<int>(4, Globals.MemoryPool, ptr => *(int*)ptr, (ptr, v) => *(int*)ptr = v))
        //    {
        //        TestArray(array);
        //    }
        //    Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0);
        //}

        public void TestArray(LargeArray<int> array)
        {
            for (int x = 0; x < 250000; x++)
            {
                if (x >= array.Capacity)
                {
                    HelperFunctions.ExpectError(() => array[x] = x);
                    array.SetCapacity(array.Capacity + 1);
                }
                array[x] = x;
            }

            for (int x = 0; x < 250000; x++)
            {
                Assert.AreEqual(array[x], x);
            }
        }

    }
}