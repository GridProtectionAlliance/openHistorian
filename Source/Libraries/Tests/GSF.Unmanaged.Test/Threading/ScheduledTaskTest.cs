using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace GSF.Threading.Test
{
    [TestFixture]
    public class ScheduledTaskTest
    {
        [Test]
        public void TestDisposed()
        {
            int count = 0;
            ScheduledTask worker = new ScheduledTask(ThreadingMode.Foreground);
            WeakReference workerWeak = new WeakReference(worker);
            worker = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            worker = (ScheduledTask)workerWeak.Target;
            Assert.IsNull(worker);
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
            public readonly ScheduledTask worker;

            public NestedDispose()
            {
                worker = new ScheduledTask(ThreadingMode.Foreground);
                worker.OnEvent += Method;
            }

            private void Method(object sender, ScheduledTaskEventArgs scheduledTaskEventArgs)
            {
            }
        }


        [Test]
        public void Test()
        {
            using (ScheduledTask work = new ScheduledTask(ThreadingMode.Foreground))
            {
                work.OnRunWorker += work_DoWork;
                work.OnDispose += work_CleanupWork;
                work.Start();
            }
            Debugger.Break();
            double x = 1;
            while (x > 3)
            {
                x--;
            }
        }

        private void work_CleanupWork(object sender, ScheduledTaskEventArgs scheduledTaskEventArgs)
        {
            Thread.Sleep(100);
        }

        private void work_DoWork(object sender, ScheduledTaskEventArgs scheduledTaskEventArgs)
        {
            Thread.Sleep(100);
        }
    }
}