using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace openHistorian.Core.Unmanaged
{
    static class BufferPoolTest
    {

        static List<int> lst;
        public static void Test()
        {
            Stopwatch sw = new Stopwatch();
            BufferPool.RequestCollection += new Action<BufferPoolCollectionMode>(BufferPool_RequestCollection);
            long memory = BufferPool.SystemTotalPhysicalMemory;
            if (!BufferPool.IsUsingLargePageSizes)
                throw new Exception();
            long minMem = BufferPool.MinimumMemoryUsage;
            long maxMemory = BufferPool.MaximumMemoryUsage;

            if (memory == 1 || minMem == 1 || maxMemory == 1)
                memory = memory;
            BufferPool.SetMinimumMemoryUsage(long.MaxValue);
            BufferPool.SetMaximumMemoryUsage(long.MaxValue);
            sw.Start();
            lst = new List<int>(1000000);
                for (int x = 0; x < 10000000; x++)
                {
                    IntPtr ptr;
                    lst.Add(BufferPool.AllocatePage(out ptr));
                }
                foreach (int x in lst)
                {
                    BufferPool.ReleasePage(x);
                }
            sw.Stop();
            MessageBox.Show((10000000/sw.Elapsed.TotalSeconds/1000000).ToString());
            lst.Clear();
        }

        static void BufferPool_RequestCollection(BufferPoolCollectionMode obj)
        {
            if (obj == BufferPoolCollectionMode.Critical)
            {
                int ItemsToRemove = lst.Count / 5;
                while (lst.Count > ItemsToRemove)
                {
                    BufferPool.ReleasePage(lst[lst.Count-1]);
                    lst.RemoveAt(lst.Count-1);
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
