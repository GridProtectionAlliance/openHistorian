using System;
using System.Diagnostics;
using System.Threading;
using GSF.Threading;
using NUnit.Framework;

namespace openHistorian.PerformanceTests.Threading
{
    [TestFixture]
    public class SmallLock_Test
    {
        [Test]
        public void TestMonitor()
        {
            const int count = 100000000;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            object obj = new object();

            for (int x = 0; x < count; x++)
            {
                lock (obj) ;
                lock (obj) ;
                lock (obj) ;
                lock (obj) ;
                lock (obj) ;
                lock (obj) ;
                lock (obj) ;
                lock (obj) ;
                lock (obj) ;
                lock (obj) ;
            }
            sw.Stop();

            Console.WriteLine((count * 10.0 / sw.Elapsed.TotalSeconds / 1000000));
        }

        [Test]
        public void TestSmallLock_Lock()
        {
            var tl = new MonitorHelper(new object(), false);
            const int count = 100000000;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int x = 0; x < count; x++)
            {
                tl.Enter(); tl.Exit();
                tl.Enter(); tl.Exit();
                tl.Enter(); tl.Exit();
                tl.Enter(); tl.Exit();
                tl.Enter(); tl.Exit();
                tl.Enter(); tl.Exit();
                tl.Enter(); tl.Exit();
                tl.Enter(); tl.Exit();
                tl.Enter(); tl.Exit();
                tl.Enter(); tl.Exit();
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
               
            }
            sw.Stop();

            Console.WriteLine((count * 10.0 / sw.Elapsed.TotalSeconds / 1000000));
        }

        [Test]
        public void TestSmallLock2_Lock()
        {
            var tl = new MonitorHelper(new object(),false);
            const int count = 100000000;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int x = 0; x < count; x++)
            {
                if (tl.TryEnter()) tl.Exit();
                if (tl.TryEnter()) tl.Exit();
                if (tl.TryEnter()) tl.Exit();
                if (tl.TryEnter()) tl.Exit();
                if (tl.TryEnter()) tl.Exit();
                if (tl.TryEnter()) tl.Exit();
                if (tl.TryEnter()) tl.Exit();
                if (tl.TryEnter()) tl.Exit();
                if (tl.TryEnter()) tl.Exit();
                if (tl.TryEnter()) tl.Exit();
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;
                //using (tl.Enter()) ;

            }
            sw.Stop();

            Console.WriteLine((count * 10.0 / sw.Elapsed.TotalSeconds / 1000000));
        }

        ManualResetEvent m_event;
        TinyLock m_sync;
        long m_value;
        const long max = 100000000;

        [Test]
        public void TestContention()
        {
            m_value = 0;
            m_sync=new TinyLock();
            m_event=new ManualResetEvent(true);

            for (int x = 0; x < 16; x++)
                ThreadPool.QueueUserWorkItem(Adder);

            Thread.Sleep(100);
            m_event.Set();

            while (m_value < 16 * max)
            {
                Console.WriteLine(m_value);
                Thread.Sleep(1000);
            }

            Console.WriteLine(m_value);
        }

        public void Adder(object obj)
        {
            m_event.WaitOne();
            for (int x = 0; x < max; x++)
            {
                using (m_sync.Lock())
                    m_value++;
            }
        }
    }
}
