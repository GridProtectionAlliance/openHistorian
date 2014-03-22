using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GSF.IO.Unmanaged;
using GSF.Threading;
using NUnit.Framework;

namespace openHistorian.PerformanceTests.Threading
{
    [TestFixture]
    class ThreadContainerBase_Test
    {
        int m_doWorkCount;

        [Test]
        public void Test()
        {
            Action<ThreadContainerBase.CallbackArgs> callback = Callback;
            object o;
            Test(new ThreadContainerDedicated(new WeakActionFast<ThreadContainerBase.CallbackArgs>(callback, out o), false, ThreadPriority.Normal), ThreadingMode.DedicatedForeground);
            Test(new ThreadContainerDedicated(new WeakActionFast<ThreadContainerBase.CallbackArgs>(callback, out o), true, ThreadPriority.Normal), ThreadingMode.DedicatedBackground);
            Test(new ThreadContainerThreadpool(new WeakActionFast<ThreadContainerBase.CallbackArgs>(callback, out o)), ThreadingMode.ThreadPool);
        }

        void Callback(ThreadContainerBase.CallbackArgs callbackArgs)
        {
            m_doWorkCount++;
        }

        void Test(ThreadContainerBase work, ThreadingMode mode)
        {
            const int Count = 1000000000;
            Stopwatch sw = new Stopwatch();
            m_doWorkCount = 0;
            sw.Start();
            for (int x = 0; x < 1000; x++)
                work.Start();

            sw.Stop();
            Thread.Sleep(1000);
            m_doWorkCount = 0;
            sw.Reset();

            sw.Start();
            for (int x = 0; x < Count; x++)
                work.Start();

            sw.Stop();

            Console.WriteLine(mode.ToString());
            Console.WriteLine(" Fire Event Count: " + m_doWorkCount.ToString());
            Console.WriteLine("  Fire Event Rate: " + (m_doWorkCount / sw.Elapsed.TotalSeconds / 1000000).ToString("0.00"));
            Console.WriteLine(" Total Calls Time: " + sw.Elapsed.TotalMilliseconds.ToString("0.0") + "ms");
            Console.WriteLine(" Total Calls Rate: " + (Count / sw.Elapsed.TotalSeconds / 1000000).ToString("0.00"));
            Console.WriteLine();
        }

        [Test]
        public void TestTimed()
        {
            Action<ThreadContainerBase.CallbackArgs> callback = Callback;
            object o;
            Test(new ThreadContainerDedicated(new WeakActionFast<ThreadContainerBase.CallbackArgs>(callback, out o), false, ThreadPriority.Normal), ThreadingMode.DedicatedForeground);
            Test(new ThreadContainerDedicated(new WeakActionFast<ThreadContainerBase.CallbackArgs>(callback, out o), true, ThreadPriority.Normal), ThreadingMode.DedicatedBackground);
            Test(new ThreadContainerThreadpool(new WeakActionFast<ThreadContainerBase.CallbackArgs>(callback, out o)), ThreadingMode.ThreadPool);
        }

        void TestTimed(ThreadContainerBase work, ThreadingMode mode)
        {
            const int Count = 100000000;
            Stopwatch sw = new Stopwatch();
            m_doWorkCount = 0;
            sw.Start();
            for (int x = 0; x < 1000; x++)
            {
                work.Start(10);
                work.Start();
            }

            sw.Stop();
            Thread.Sleep(1000);
            m_doWorkCount = 0;
            sw.Reset();

            sw.Start();
            for (int x = 0; x < Count; x++)
            {
                work.Start(10);
                work.Start();
            }

            sw.Stop();

            Console.WriteLine(mode.ToString());
            Console.WriteLine(" Fire Event Count: " + m_doWorkCount.ToString());
            Console.WriteLine("  Fire Event Rate: " + (m_doWorkCount / sw.Elapsed.TotalSeconds / 1000000).ToString("0.00"));
            Console.WriteLine(" Total Calls Time: " + sw.Elapsed.TotalMilliseconds.ToString("0.0") + "ms");
            Console.WriteLine(" Total Calls Rate: " + (Count / sw.Elapsed.TotalSeconds / 1000000).ToString("0.00"));
            Console.WriteLine();
        }

        [Test]
        public void TestConcurrent()
        {
            TestConcurrent(ThreadingMode.DedicatedBackground);
            TestConcurrent(ThreadingMode.DedicatedForeground);
            TestConcurrent(ThreadingMode.ThreadPool);

        }

        void TestConcurrent(ThreadingMode mode)
        {
            int workCount;

            const int Count = 100000000;
            Stopwatch sw = new Stopwatch();
            m_doWorkCount = 0;
            using (ScheduledTask work = new ScheduledTask(mode))
            {
                work.OnRunning += work_DoWork;

                sw.Start();
                for (int x = 0; x < 1000; x++)
                    work.Start();

                sw.Stop();
            }
            m_doWorkCount = 0;
            sw.Reset();

            using (ScheduledTask work = new ScheduledTask(mode))
            {
                work.OnRunning += work_DoWork;


                sw.Start();
                ThreadPool.QueueUserWorkItem(BlastStartMethod, work);
                ThreadPool.QueueUserWorkItem(BlastStartMethod, work);

                for (int x = 0; x < Count; x++)
                    work.Start();
                workCount = m_doWorkCount;
                sw.Stop();
                Thread.Sleep(100);
            }

            Console.WriteLine(mode.ToString());
            Console.WriteLine(" Fire Event Count: " + workCount.ToString());
            Console.WriteLine("  Fire Event Rate: " + (workCount / sw.Elapsed.TotalSeconds / 1000000).ToString("0.00"));
            Console.WriteLine(" Total Calls Time: " + sw.Elapsed.TotalMilliseconds.ToString("0.0") + "ms");
            Console.WriteLine(" Total Calls Rate: " + (Count / sw.Elapsed.TotalSeconds / 1000000).ToString("0.00"));
            Console.WriteLine();
        }


        void BlastStartMethod(object obj)
        {
            try
            {
                ScheduledTask task = (ScheduledTask)obj;
                const int Count = 100000000;
                for (int x = 0; x < Count; x++)
                    task.Start();
            }
            catch (Exception)
            {

            }
        }


        private void work_DoWork(ThreadContainerCallbackReason threadContainerCallbackReason)
        {
            m_doWorkCount++;
        }

    }
}
