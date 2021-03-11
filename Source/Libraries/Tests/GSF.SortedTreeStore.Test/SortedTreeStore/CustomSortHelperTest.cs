//******************************************************************************************************
//  CustomSortHelperTest.cs - Gbtc
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
//  10/26/2013 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using GSF.IO.Unmanaged.Test;
using NUnit.Framework;

namespace GSF.Snap.Test
{
    [TestFixture]
    public class CustomSortHelperTest
    {
        [Test]
        public void Test1()
        {
            MemoryPoolTest.TestMemoryLeak();
            Test(0);
            Test(1);
            Test(2);
            Test(100);
            Test(1000);
            MemoryPoolTest.TestMemoryLeak();
        }

        [Test]
        public void Test2()
        {
            MemoryPoolTest.TestMemoryLeak();
            TestWithRenumber(0);
            TestWithRenumber(1);
            TestWithRenumber(2);
            TestWithRenumber(100);
            TestWithRenumber(1000);
            MemoryPoolTest.TestMemoryLeak();
        }

        public void Test(int count)
        {
            List<int> correctList = new List<int>();
            Random r = new Random();
            for (int x = 0; x < count; x++)
            {
                correctList.Add(r.Next(1000000000));
            }
            CustomSortHelper<int> items = new CustomSortHelper<int>(correctList, (x, y) => x.CompareTo(y) < 0);
            correctList.Sort();
            for (int x = 0; x < count; x++)
                if (correctList[x] != items[x])
                    throw new Exception();
        }

        public void TestWithRenumber(int count)
        {
            List<int> correctList = new List<int>();
            Random r = new Random();
            for (int x = 0; x < count; x++)
            {
                correctList.Add(r.Next(10000000));
            }
            CustomSortHelper<int> items = new CustomSortHelper<int>(correctList, (x, y) => x.CompareTo(y) < 0);
            correctList.Sort();

            for (int i = 0; i < Math.Min(count, 100); i++)
            {
                int adder = r.Next(10000000);
                correctList[i] += adder;
                items[i] += adder;
                correctList.Sort();
                items.SortAssumingIncreased(i);

                for (int x = 0; x < count; x++)
                    if (correctList[x] != items[x])
                        throw new Exception();

            }

        }
    }
}
