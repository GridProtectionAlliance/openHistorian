//using System;
//using System.Diagnostics;
//using System.Threading;
//using NUnit.Framework;

//namespace GSF.Threading.Test
//{
//    [TestFixture]
//    public class WeakWorkerThreadTest
//    {
//        bool shouldDispose = false;
//        [Test]
//        public void TestDisposed()
//        {
//            int count = 0;
//            var worker = new WeakWorkerThread(Callback, false, ThreadPriority.Normal);
//            WeakReference workerWeak = new WeakReference(worker);
//            worker = null;
//            GC.Collect();
//            GC.WaitForPendingFinalizers();
//            worker = (WeakWorkerThread)workerWeak.Target;
//            Assert.IsNull(worker);
//        }

//        void Callback(WorkerThreadTimeoutResults executionMode, ref bool shouldDispose)
//        {
//            shouldDispose = this.shouldDispose;
//            Console.WriteLine(executionMode.ToString());
//        }

//        [Test]
//        public void TestDisposedNested()
//        {
//            int count = 0;
//            NestedDispose worker = new NestedDispose();
//            WeakReference workerWeak = new WeakReference(worker);
//            worker = null;
//            GC.Collect();
//            GC.WaitForPendingFinalizers();
//            worker = (NestedDispose)workerWeak.Target;
//            Assert.IsNull(worker);
//        }


//        private class NestedDispose
//        {
//            public readonly WeakWorkerThread worker;

//            public NestedDispose()
//            {
//                worker = new WeakWorkerThread(Callback, false, ThreadPriority.Normal);
//            }

//            void Callback(WorkerThreadTimeoutResults executionMode, ref bool shouldDispose)
//            {
//                Console.WriteLine(executionMode.ToString());
//            }

//        }


//        [Test]
//        public void Test()
//        {
//            using (var work = new WeakWorkerThread(Callback, false, ThreadPriority.Normal))
//            {
//                work.Start();
//                Thread.Sleep(1);
//                work.Start(1);
//                Thread.Sleep(5);
//                work.Start(1);
//                work.Start();
//                Thread.Sleep(10);
//                shouldDispose = true;
//                work.Start(1);
//            }

//        }

//    }
//}