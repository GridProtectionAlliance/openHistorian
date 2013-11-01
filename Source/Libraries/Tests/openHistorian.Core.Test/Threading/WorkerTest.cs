using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace GSF.Threading.Test
{
    [TestFixture]
    public class WorkerTest
    {
        [Test]
        public void RunAfterDelay()
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            Stopwatch sw3 = new Stopwatch();
            Stopwatch sw4 = new Stopwatch();
            Stopwatch sw5 = new Stopwatch();
            sw1.Reset();
            sw2.Reset();
            sw3.Reset();
            sw4.Reset();
            sw5.Reset();

            //Primes the oprations.
            Worker.RunAfterDelay(0, sw1.Reset);
            Thread.Sleep(100);

            sw1.Start();
            sw2.Start();
            sw3.Start();
            sw4.Start();
            sw5.Start();
            Worker.RunAfterDelay(0, sw2.Stop);
            Worker.RunAfterDelay(100, sw3.Stop);
            Worker.RunAfterDelay(10, sw4.Stop);
            Worker.RunAfterDelay(1000, sw5.Stop);
            sw1.Stop();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(1100);
            Assert.AreEqual(false, sw1.IsRunning);
            Assert.AreEqual(false, sw2.IsRunning);
            Assert.AreEqual(false, sw3.IsRunning);
            Assert.AreEqual(false, sw4.IsRunning);
            Assert.AreEqual(false, sw5.IsRunning);

            Assert.Less(0.0 - 15, sw1.Elapsed.TotalMilliseconds);
            Assert.Greater(0.0 + 15, sw1.Elapsed.TotalMilliseconds);

            Assert.Less(10.0 - 15, sw2.Elapsed.TotalMilliseconds);
            Assert.Greater(1.0 + 15, sw2.Elapsed.TotalMilliseconds);

            Assert.Less(100.0 - 15, sw3.Elapsed.TotalMilliseconds);
            Assert.Greater(100.0 + 15, sw3.Elapsed.TotalMilliseconds);

            Assert.Less(10.0 - 15, sw4.Elapsed.TotalMilliseconds);
            Assert.Greater(10.0 + 15, sw4.Elapsed.TotalMilliseconds);

            Assert.Less(1000.0 - 15, sw5.Elapsed.TotalMilliseconds);
            Assert.Greater(1000.0 + 15, sw5.Elapsed.TotalMilliseconds);
        }
    }
}