//******************************************************************************************************
//  NullableLargeArrayTest.cs - Gbtc
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

using System;
using System.Linq;
using GSF.IO.Unmanaged.Test;
using NUnit.Framework;
using openHistorian;

namespace GSF.Collections.Test
{
    [TestFixture]
    public class NullableLargeArrayTest
    {
        [Test]
        public void TestLargeArray()
        {
            MemoryPoolTest.TestMemoryLeak();
            TestArray(new NullableLargeArray<int>());
            MemoryPoolTest.TestMemoryLeak();
        }

        public void TestArray(NullableLargeArray<int> array)
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


        [Test]
        public void TestCount()
        {
            NullableLargeArray<int> array = new NullableLargeArray<int>();

            for (int x = 0; x < 100000; x++)
            {
                array.AddValue(x);
            }

            for (int x = 0; x < 100000; x += 2)
            {
                if (array[x] != x)
                    throw new Exception();
                array.SetNull(x);
            }

            if (array.Capacity != 100000 + (1024 - (100000 & 1023)))
                throw new Exception();
            if (array.CountUsed != 50000)
                throw new Exception();
            if (array.CountFree != array.Capacity - 50000)
                throw new Exception();


            int i;
            for (int x = 1; x < 100000; x += 2)
            {
                if (!array.TryGetValue(x, out i))
                    throw new Exception();
                if (i != x)
                    throw new Exception();
            }

            for (int x = 0; x < 100000; x += 2)
            {
                if (array.TryGetValue(x, out i))
                    throw new Exception();
            }

            if (array.Count() != 50000)
                throw new Exception();

            i = 1;
            foreach (int item in array)
            {
                if (item != i)
                    throw new Exception();
                i += 2;
            }

            if (array.Select(x => x).Count() != 50000)
                throw new Exception();


            for (int x = 1; x < 100000; x += 2)
            {
                array.OverwriteValue(x, -1);
            }

            for (int x = 1; x < 100000; x += 2)
            {
                if (array[x] != -1)
                    throw new Exception();
            }

        }



    }
}