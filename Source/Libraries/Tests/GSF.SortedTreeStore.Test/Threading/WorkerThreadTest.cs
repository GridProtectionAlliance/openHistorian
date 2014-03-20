using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace GSF.Threading.Test
{
    [TestFixture]
    public class WorkerThreadTest
    {
        [Test]
        public void TestDisposed()
        {
            int count = 0;
            var worker = new WorkerThread(Callback, true, false, ThreadPriority.Normal);
            WeakReference workerWeak = new WeakReference(worker);
            worker = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            worker = (WorkerThread)workerWeak.Target;
            Assert.IsNull(worker);
        }

        void Callback(WorkerThreadTimeoutResults executionMode)
        {
            Console.WriteLine(executionMode.ToString());
        }

        [Test]
        public void TestDisposedNested()
        {
            int count = 0;
            NestedDispose worker = new NestedDispose();
            WeakReference workerWeak = new WeakReference(worker);
            worker = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            worker = (NestedDispose)workerWeak.Target;
            Assert.IsNull(worker);
        }

        private class NestedDispose
        {
            public readonly WorkerThread worker;

            public NestedDispose()
            {
                worker = new WorkerThread(Callback, true, false, ThreadPriority.Normal);
            }

            void Callback(WorkerThreadTimeoutResults executionMode)
            {
                Console.WriteLine(executionMode.ToString());
            }

        }

        [Test]
        public void Test()
        {
            using (var work = new WorkerThread(Callback, true, false, ThreadPriority.Normal))
            {
                work.Start();
                Thread.Sleep(100);
                work.Start(10);
                Thread.Sleep(100);
                work.Start(50);
                work.Start();
                Thread.Sleep(100);
                work.Start(10);
                work.StartOnMyThread();
            }
        }

        [Test]
        public void TestSignalRate()
        {
            const long Calls = 500 * 1000000;
            m_callbackCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var work = new WorkerThread(Callback2, true, false, ThreadPriority.Normal))
            {
                for (int x = 0; x < Calls; x++)
                {
                    work.Start();
                    work.Start();
                    work.Start();
                    work.Start();
                    work.Start();
                    work.Start();
                    work.Start();
                    work.Start();
                    work.Start();
                    work.Start();
                }
            }
            sw.Stop();
            Console.WriteLine(m_callbackCount);
            Console.WriteLine(sw.Elapsed.TotalSeconds);
            Console.WriteLine("Call Rate: " + (Calls * 10.0 / sw.Elapsed.TotalSeconds / 1000000).ToString());
            Console.WriteLine("Callback Rate: " + (m_callbackCount / sw.Elapsed.TotalSeconds / 1000000).ToString());
        }

        [Test]
        public void TestSignalRateMyThread()
        {
            const long Calls = 10 * 1000000;
            m_callbackCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var work = new WorkerThread(Callback2, true, false, ThreadPriority.Normal))
            {
                for (int x = 0; x < Calls; x++)
                {
                    work.StartOnMyThread();
                    work.StartOnMyThread();
                    work.StartOnMyThread();
                    work.StartOnMyThread();
                    work.StartOnMyThread();
                    work.StartOnMyThread();
                    work.StartOnMyThread();
                    work.StartOnMyThread();
                    work.StartOnMyThread();
                }
            }
            sw.Stop();
            Console.WriteLine(m_callbackCount);
            Console.WriteLine(sw.Elapsed.TotalSeconds);
            Console.WriteLine("Call Rate: " + (Calls * 10.0 / sw.Elapsed.TotalSeconds / 1000000).ToString());
            Console.WriteLine("Callback Rate: " + (m_callbackCount / sw.Elapsed.TotalSeconds / 1000000).ToString());
        }

        [Test]
        public void TestSignalRateDelay()
        {
            const long Calls = 500 * 1000000;
            m_callbackCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var work = new WorkerThread(Callback2, true, false, ThreadPriority.Normal))
            {
                for (int x = 0; x < Calls; x++)
                {
                    work.Start(1);
                    work.Start(1);
                    work.Start(1);
                    work.Start(1);
                    work.Start(1);
                    work.Start(1);
                    work.Start(1);
                    work.Start(1);
                    work.Start(1);
                    work.Start(1);
                }
            }
            sw.Stop();
            Console.WriteLine(m_callbackCount);
            Console.WriteLine(sw.Elapsed.TotalSeconds);
            Console.WriteLine("Call Rate: " + (Calls * 10.0 / sw.Elapsed.TotalSeconds / 1000000).ToString());
            Console.WriteLine("Callback Rate: " + (m_callbackCount / sw.Elapsed.TotalSeconds).ToString());
        }

        [Test]
        public void TestSignalRateDelay10()
        {
            const long Calls = 500 * 1000000;
            m_callbackCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var work = new WorkerThread(Callback2, true, false, ThreadPriority.Normal))
            {
                for (int x = 0; x < Calls; x++)
                {
                    work.Start(10);
                    work.Start(10);
                    work.Start(10);
                    work.Start(10);
                    work.Start(10);
                    work.Start(10);
                    work.Start(10);
                    work.Start(10);
                    work.Start(10);
                    work.Start(10);
                }
            }
            sw.Stop();
            Console.WriteLine(m_callbackCount);
            Console.WriteLine(sw.Elapsed.TotalSeconds);
            Console.WriteLine("Call Rate: " + (Calls * 10.0 / sw.Elapsed.TotalSeconds / 1000000).ToString());
            Console.WriteLine("Callback Rate: " + (m_callbackCount / sw.Elapsed.TotalSeconds).ToString());
        }

        long m_callbackCount;

        void Callback2(WorkerThreadTimeoutResults executionMode)
        {
            m_callbackCount++;
        }


    }
}