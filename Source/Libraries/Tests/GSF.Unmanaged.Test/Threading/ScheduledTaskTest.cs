using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace GSF.Threading
{
    [TestFixture]
    public class ScheduledTaskTest
    {
        [Test]
        public void TestDisposed()
        {
            int count = 0;
            var worker = new ScheduledTask(ThreadingMode.Foreground);
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
            var worker = new NestedDispose();
            WeakReference workerWeak = new WeakReference(worker);
            worker = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            worker = (NestedDispose)workerWeak.Target;
            Assert.IsNull(worker);
        }


        class NestedDispose
        {
            public ScheduledTask worker;

            public NestedDispose()
            {
                worker = new ScheduledTask(ThreadingMode.Foreground);
                worker.OnEvent+=Method;
            }

            void Method(object sender, ScheduledTaskEventArgs scheduledTaskEventArgs)
            {
                
            }
        }


        [Test]
        public void Test()
        {
            using (var work = new ScheduledTask(ThreadingMode.Foreground))
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

        void work_CleanupWork(object sender, ScheduledTaskEventArgs scheduledTaskEventArgs)
        {
            Thread.Sleep(100);
        }

        void work_DoWork(object sender, ScheduledTaskEventArgs scheduledTaskEventArgs)
        {
            Thread.Sleep(100);
        }

    }
}
