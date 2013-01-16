//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;

//namespace GSF.Threading
//{
//    [TestFixture]
//    public class AsyncWorkerEventArgsTest
//    {
//        [Test]
//        public void Test()
//        {
//            var args = new AsyncWorkerEventArgs();
//            Assert.AreEqual(false, args.ShouldRunAgainAfterDelay);
//            Assert.AreEqual(false, args.ShouldRunAgainImmediately);
//            Assert.AreEqual(false, args.RunAgainDelay.HasValue);
//            Assert.AreEqual(-1, args.WaitHandleTime);

//            args.RunAgain();
//            Assert.AreEqual(false, args.ShouldRunAgainAfterDelay);
//            Assert.AreEqual(true, args.ShouldRunAgainImmediately);
//            Assert.AreEqual(false, args.RunAgainDelay.HasValue);
//            Assert.AreEqual(0, args.WaitHandleTime);

//            args.RunAgainAfterDelay(1);
//            Assert.AreEqual(false, args.ShouldRunAgainAfterDelay);
//            Assert.AreEqual(true, args.ShouldRunAgainImmediately);
//            Assert.AreEqual(false, args.RunAgainDelay.HasValue);
//            Assert.AreEqual(0, args.WaitHandleTime);

//            args.RunAgain();
//            Assert.AreEqual(false, args.ShouldRunAgainAfterDelay);
//            Assert.AreEqual(true, args.ShouldRunAgainImmediately);
//            Assert.AreEqual(false, args.RunAgainDelay.HasValue);
//            Assert.AreEqual(0, args.WaitHandleTime);
//        }

//        [Test]
//        public void Test2()
//        {
//            var args = new AsyncWorkerEventArgs();
//            Assert.AreEqual(false, args.ShouldRunAgainAfterDelay);
//            Assert.AreEqual(false, args.ShouldRunAgainImmediately);
//            Assert.AreEqual(false, args.RunAgainDelay.HasValue);
//            Assert.AreEqual(-1, args.WaitHandleTime);

//            args.RunAgainAfterDelay(100);
//            Assert.AreEqual(true, args.ShouldRunAgainAfterDelay);
//            Assert.AreEqual(false, args.ShouldRunAgainImmediately);
//            Assert.AreEqual(true, args.RunAgainDelay.HasValue);
//            Assert.AreEqual(TimeSpan.TicksPerMillisecond * 100, args.RunAgainDelay.Value.Ticks);
//            Assert.AreEqual(100, args.WaitHandleTime);

//            args.RunAgainAfterDelay(99);
//            Assert.AreEqual(true, args.ShouldRunAgainAfterDelay);
//            Assert.AreEqual(false, args.ShouldRunAgainImmediately);
//            Assert.AreEqual(true, args.RunAgainDelay.HasValue);
//            Assert.AreEqual(TimeSpan.TicksPerMillisecond * 99, args.RunAgainDelay.Value.Ticks);
//            Assert.AreEqual(99, args.WaitHandleTime);

//            args.RunAgainAfterDelay(101);
//            Assert.AreEqual(true, args.ShouldRunAgainAfterDelay);
//            Assert.AreEqual(false, args.ShouldRunAgainImmediately);
//            Assert.AreEqual(true, args.RunAgainDelay.HasValue);
//            Assert.AreEqual(TimeSpan.TicksPerMillisecond * 99, args.RunAgainDelay.Value.Ticks);
//            Assert.AreEqual(99, args.WaitHandleTime);

//            args.RunAgainAfterDelay(90);
//            Assert.AreEqual(true, args.ShouldRunAgainAfterDelay);
//            Assert.AreEqual(false, args.ShouldRunAgainImmediately);
//            Assert.AreEqual(true, args.RunAgainDelay.HasValue);
//            Assert.AreEqual(TimeSpan.TicksPerMillisecond * 90, args.RunAgainDelay.Value.Ticks);
//            Assert.AreEqual(90, args.WaitHandleTime);

//            args.RunAgain();
//            Assert.AreEqual(false, args.ShouldRunAgainAfterDelay);
//            Assert.AreEqual(true, args.ShouldRunAgainImmediately);
//            Assert.AreEqual(false, args.RunAgainDelay.HasValue);
//            Assert.AreEqual(0, args.WaitHandleTime);

//            args.RunAgainAfterDelay(90);
//            Assert.AreEqual(false, args.ShouldRunAgainAfterDelay);
//            Assert.AreEqual(true, args.ShouldRunAgainImmediately);
//            Assert.AreEqual(false, args.RunAgainDelay.HasValue);
//            Assert.AreEqual(0, args.WaitHandleTime);
//        }
//    }
//}
