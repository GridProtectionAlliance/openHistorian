using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.UnmanagedMemory
{

    [TestClass()]
    public class BufferPoolTest2
    {
        [TestMethod()]
        public void TestMemoryLeak()
        {
            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);
        }
    }

    [TestClass()]
    public class BufferPoolTest
    {
        static List<int> lst;

        [TestMethod()]
        public void Test()
        {
            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);


            var del = new Action<BufferPoolCollectionMode>(BufferPool_RequestCollection);
            Globals.BufferPool.RequestCollection += del;

            //Test1();
            Test2();

            Globals.BufferPool.RequestCollection -= del;

            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0L);


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

        unsafe static void Test2()
        {
            Random random = new Random();
            int seed = random.Next();

            var lstkeep = new List<int>(1000);
            var lst = new List<int>(1000);
            var lstp = new List<IntPtr>(1000);

            for (int tryagain = 0; tryagain < 10; tryagain++)
            {
                random = new Random(seed);
                for (int x = 0; x < 1000; x++)
                {
                    IntPtr ptr;
                    int index;
                    Globals.BufferPool.AllocatePage(out index, out ptr);
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
                    Globals.BufferPool.ReleasePage(lst[x + 1]);
                }
                lst.Clear();
                lstp.Clear();
            }

            foreach (int x in lstkeep)
            {
                Globals.BufferPool.ReleasePage(x);
            }
            lst.Clear();
            lst = null;
            if (Globals.BufferPool.AllocatedBytes > 0)
                throw new Exception("");
        }

        static void BufferPool_RequestCollection(BufferPoolCollectionMode obj)
        {
            if (lst == null)
                return;
            if (obj == BufferPoolCollectionMode.Critical)
            {
                int ItemsToRemove = lst.Count / 5;
                while (lst.Count > ItemsToRemove)
                {
                    Globals.BufferPool.ReleasePage(lst[lst.Count - 1]);
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
