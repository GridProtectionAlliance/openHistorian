using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace GSF.UnmanagedMemory.Test
{
    [TestFixture()]
    public class BufferPoolTest2
    {
        [Test()]
        public void TestMemoryLeak()
        {
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);
        }
    }

    [TestFixture()]
    public class BufferPoolTest
    {
        private static List<int> lst;

        [Test()]
        public void Test()
        {
            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);


            EventHandler<CollectionEventArgs> del = new EventHandler<CollectionEventArgs>(BufferPool_RequestCollection);
            Globals.MemoryPool.RequestCollection += del;

            //Test1();
            Test2();

            Globals.MemoryPool.RequestCollection -= del;

            Assert.AreEqual(Globals.MemoryPool.AllocatedBytes, 0L);


            Assert.IsTrue(true);
        }

        //static void Test1()
        //{
        //    Stopwatch sw = new Stopwatch();
        //    long memory = BufferPool.SystemTotalPhysicalMemory;
        //    if (!BufferPool.IsUsingLargePageSizes)
        //        throw new Exception();
        //    long minMem = BufferPool.MinimumMemoryUsage;
        //    long maxMemory = BufferPool.MaximumMemoryUsage;

        //    if (memory == 1 || minMem == 1 || maxMemory == 1)
        //        memory = memory;
        //    BufferPool.SetMinimumMemoryUsage(long.MaxValue);
        //    BufferPool.SetMaximumMemoryUsage(long.MaxValue);
        //    sw.Start();
        //    lst = new List<int>(100000);
        //    for (int x = 0; x < 10000000; x++)
        //    {
        //        IntPtr ptr;
        //        lst.Add(BufferPool.AllocatePage(out ptr));
        //    }
        //    foreach (int x in lst)
        //    {
        //        BufferPool.ReleasePage(x);
        //    }
        //    sw.Stop();
        //    MessageBox.Show((10000000 / sw.Elapsed.TotalSeconds / 1000000).ToString());
        //    lst.Clear();
        //    lst = null;
        //}

        private static unsafe void Test2()
        {
            Random random = new Random();
            int seed = random.Next();

            List<int> lstkeep = new List<int>(1000);
            List<int> lst = new List<int>(1000);
            List<IntPtr> lstp = new List<IntPtr>(1000);

            for (int tryagain = 0; tryagain < 10; tryagain++)
            {
                random = new Random(seed);
                for (int x = 0; x < 1000; x++)
                {
                    IntPtr ptr;
                    int index;
                    Globals.MemoryPool.AllocatePage(out index, out ptr);
                    lst.Add(index);
                    lstp.Add(ptr);
                }

                foreach (IntPtr ptr in lstp)
                {
                    int* lp = (int*)ptr.ToPointer();
                    *lp++ = random.Next();
                    *lp++ = random.Next();
                    *lp++ = random.Next();
                    *lp++ = random.Next();
                }

                random = new Random(seed);
                foreach (IntPtr ptr in lstp)
                {
                    int* lp = (int*)ptr.ToPointer();

                    if (*lp++ != random.Next()) throw new Exception();
                    if (*lp++ != random.Next()) throw new Exception();
                    if (*lp++ != random.Next()) throw new Exception();
                    if (*lp++ != random.Next()) throw new Exception();
                }

                for (int x = 0; x < 1000; x += 2)
                {
                    lstkeep.Add(lst[x]);
                    Globals.MemoryPool.ReleasePage(lst[x + 1]);
                }
                lst.Clear();
                lstp.Clear();
            }

            foreach (int x in lstkeep)
            {
                Globals.MemoryPool.ReleasePage(x);
            }
            lst.Clear();
            lst = null;
            if (Globals.MemoryPool.AllocatedBytes > 0)
                throw new Exception("");
        }

        private static void BufferPool_RequestCollection(object sender, CollectionEventArgs eventArgs)
        {
            if (lst == null)
                return;
            if (eventArgs.CollectionMode == BufferPoolCollectionMode.Critical)
            {
                int ItemsToRemove = lst.Count / 5;
                while (lst.Count > ItemsToRemove)
                {
                    eventArgs.ReleasePage(lst[lst.Count - 1]);
                    lst.RemoveAt(lst.Count - 1);
                }
                //for (int x = 0; x<lst.Count; x+=3)
                //{
                //    BufferPool.ReleasePage(lst[x]);
                //    lst.RemoveAt(x);
                //    x -= 1;
                //}
            }
            //throw new NotImplementedException();
        }
    }
}