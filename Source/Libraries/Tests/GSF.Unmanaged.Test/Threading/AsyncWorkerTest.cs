//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using NUnit.Framework;

//namespace GSF.Threading
//{
//    [TestFixture]
//    class AsyncWorkerTest
//    {
//        [Test]
//        public void TestDisposed()
//        {
//            var worker = new AsyncWorkerForeground();
//            WeakReference workerWeak = new WeakReference(worker);
//            worker = null;
//            GC.Collect();
//            GC.WaitForPendingFinalizers();
//            worker = (AsyncWorkerForeground)workerWeak.Target;
//            Assert.IsNotNull(worker);
//            worker.Dispose();
//            worker = null;
//            GC.Collect();
//            GC.WaitForPendingFinalizers();
//            worker = (AsyncWorkerForeground)workerWeak.Target;
//            Assert.IsNull(worker);
//        }



//        [Test]
//        public void Test()
//        {
//            using (var work = new AsyncWorker())
//            {
//                work.DoWork += work_DoWork;
//                work.CleanupWork += work_CleanupWork;
//                work.RunWorker();
//            }
//            Debugger.Break();
//            double x = 1;
//            while (x > 3)
//            {
//                x--;
//            }
//        }

//        void work_CleanupWork(object sender, EventArgs e)
//        {
//            Thread.Sleep(100);
//        }

//        void work_DoWork(object sender, EventArgs e)
//        {
//            Thread.Sleep(100);
//        }

//    }
//}
