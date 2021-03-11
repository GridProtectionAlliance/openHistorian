using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
//using System.Windows.Forms;
using NUnit.Framework;

namespace openHistorian.PerformanceTests
{
    [TestFixture]
    public class GCTime
    {
        private readonly List<AClass[]> m_objects = new List<AClass[]>();
        private readonly List<FinalizableClass[]> m_objects2 = new List<FinalizableClass[]>();

        [Test]
        public void Test()
        {
            for (int x = 0; x < 100; x++)
                AddItemsAndTime();
        }

        void AddItemsAndTime()
        {
            AClass[] array = new AClass[100000];
            for (int x = 0; x < array.Length; x++)
                array[x] = new AClass();

            m_objects.Add(array);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Stopwatch sw = new Stopwatch();
            AClass swap = m_objects[0][0];
            m_objects[0][0] = m_objects[0][1];
            m_objects[0][1] = swap;
            m_objects[0][m_objects.Count] = null;

            sw.Start();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            sw.Stop();
            long memorySize = Process.GetCurrentProcess().VirtualMemorySize64;//GC.GetTotalMemory(false);
            Console.WriteLine("{0}00k items: {1}ms  {2}", m_objects.Count.ToString(), sw.Elapsed.TotalMilliseconds.ToString("0.00"),(memorySize/1024.0/1024.0).ToString("0.0MB"));
        }

        private class AClass
        {
            public int Value = 1;
        }

        private class FinalizableClass
        {
            public readonly int Value = 1;

            ~FinalizableClass()
            {
                if (Value == int.MaxValue)
                {
                    Marshal.FreeHGlobal(Marshal.AllocHGlobal(10));
                }
            }
        }

        [Test]
        public void Test2()
        {
            for (int x = 0; x < 100; x++)
                AddItemsAndTime2();
        }

        void AddItemsAndTime2()
        {
            FinalizableClass[] array = new FinalizableClass[100000];
            for (int x = 0; x < array.Length; x++)
                array[x] = new FinalizableClass();

            m_objects2.Add(array);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Stopwatch sw = new Stopwatch();
            FinalizableClass swap = m_objects2[0][0];
            m_objects2[0][0] = m_objects2[0][1];
            m_objects2[0][1] = swap;
            m_objects2[0][m_objects2.Count] = null;

            sw.Start();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            sw.Stop();
            long memorySize = Process.GetCurrentProcess().VirtualMemorySize64;//GC.GetTotalMemory(false);
            //long memorySize = Process.GetCurrentProcess().PrivateMemorySize64;//GC.GetTotalMemory(false);
            Console.WriteLine("{0}00k items: {1}ms  {2}", m_objects2.Count.ToString(), sw.Elapsed.TotalMilliseconds.ToString("0.00"), (memorySize / 1024.0 / 1024.0).ToString("0.0MB"));
        }

    }
}
