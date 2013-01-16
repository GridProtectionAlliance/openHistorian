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
    public class AsyncWorkerForegroundTest
    {
        [Test]
        public void Test()
        {
            using (var work = new ScheduledTask(work_DoWork, work_CleanupWork))
            {
                work.Start();
            }
            Debugger.Break();
            double x = 1;
            while (x > 3)
            {
                x--;
            }
        }

        void work_CleanupWork()
        {
            Thread.Sleep(100);
        }

        void work_DoWork()
        {
            Thread.Sleep(100);
        }

    }
}
