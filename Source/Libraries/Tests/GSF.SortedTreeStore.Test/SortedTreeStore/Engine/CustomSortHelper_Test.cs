using System;
using System.Collections.Generic;
using GSF.IO.Unmanaged;
using GSF.IO.Unmanaged.Test;
using NUnit.Framework;

namespace GSF.SortedTreeStore.Server
{
    [TestFixture]
    public class CustomSortHelper_Test
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
            CustomSortHelper<int> items = new CustomSortHelper<int>(correctList, (x, y) => x.CompareTo(y));
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
            CustomSortHelper<int> items = new CustomSortHelper<int>(correctList, (x, y) => x.CompareTo(y));
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
