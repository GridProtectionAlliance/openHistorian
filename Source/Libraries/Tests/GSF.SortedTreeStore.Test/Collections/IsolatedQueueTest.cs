using System;
using System.Diagnostics;
using System.Threading;
using GSF.IO.Unmanaged;
using NUnit.Framework;

namespace GSF.Collections.Test
{
    [TestFixture]
    public class IsolatedQueueTest
    {
        private IsolatedQueue<int> m_collection;
        private const int cnt = 1000000;
        private ManualResetEvent m_wait;

        [Test]
        public void Test()
        {
            MemoryPoolTest.TestMemoryLeak();
            m_wait = new ManualResetEvent(false);
            m_collection = new IsolatedQueue<int>();

            ThreadPool.QueueUserWorkItem(RunTwo);
            m_wait.WaitOne();
            m_wait.Reset();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int x = 0; x < cnt; x++)
            {
                if (x % 10000 == 0) Thread.Sleep(1);
                m_collection.Enqueue(x);
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalSeconds);
            m_wait.WaitOne();
            MemoryPoolTest.TestMemoryLeak();
        }

        private void RunTwo(object state)
        {
            m_wait.Set();
            SpinWait wait = new SpinWait();
            //Thread.Sleep(1000);
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int value = 0;
            int count = 0;
            int countRepeats = 0;
            while (count < cnt)
            {
                while (m_collection.TryDequeue(out value))
                {
                    Assert.AreEqual(count, value);
                    count++;
                }
                countRepeats++;
                Thread.Sleep(1);
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed.TotalSeconds);
            Console.WriteLine(countRepeats);
            m_wait.Set();
            Assert.AreEqual(count, cnt);
        }
    }
}