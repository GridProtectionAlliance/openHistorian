//using System.Collections.Generic;
//using System.Linq;
//using GSF.IO.Unmanaged;
//using NUnit.Framework;

//namespace GSF.Collections.Test
//{
//    [TestFixture]
//    public class ContinuousQueueTest
//    {
//        [Test]
//        public void TestQueue()
//        {
//            Queue<int> baseLine = new Queue<int>();
//            ContinuousQueue<int> queue = new ContinuousQueue<int>();

//            for (int x = 1; x < 10000; x++)
//            {
//                Assert.IsTrue(baseLine.SequenceEqual(queue));

//                if (x % 100 == 0)
//                {
//                    Assert.AreEqual(baseLine.Dequeue(), queue.Dequeue());
//                }

//                queue.Enqueue(x);
//                baseLine.Enqueue(x);

//                Assert.AreEqual(x / 100, queue.TailIndex);
//                Assert.AreEqual(x - 1, queue.HeadIndex);
//                Assert.AreEqual(baseLine.Count, queue.Count);
//            }

//            while (queue.Count > 0)
//            {
//                Assert.AreEqual(baseLine.Count, queue.Count);
//                Assert.AreEqual(baseLine.Dequeue(), queue.Dequeue());
//                Assert.IsTrue(baseLine.SequenceEqual(queue));
//            }
//        }

//        [Test]
//        public void Test2()
//        {
//            MemoryPoolTest.TestMemoryLeak();
//            ContinuousQueue<int> queue = new ContinuousQueue<int>();

//            for (int x = 0; x < 1000; x++)
//            {
//                queue.Enqueue(x);
//            }

//            for (int x = 0; x < 1000; x++)
//            {
//                Assert.AreEqual(x, queue[x]);
//            }
//            MemoryPoolTest.TestMemoryLeak();
//        }

//        [Test]
//        public void TestGet()
//        {
//            MemoryPoolTest.TestMemoryLeak();
//            ContinuousQueue<int> queue = new ContinuousQueue<int>();

//            for (int x = 0; x < 1000; x++)
//            {
//                queue.Enqueue(x);
//            }

//            for (int x = 0; x < 500; x++)
//            {
//                queue.Dequeue();
//            }

//            for (int x = 1000; x < 1400; x++)
//            {
//                queue.Enqueue(x);
//            }

//            for (long x = queue.TailIndex; x <= queue.HeadIndex; x++)
//            {
//                Assert.AreEqual((int)x, queue[x]);
//            }
//            MemoryPoolTest.TestMemoryLeak();
//        }


//        [Test]
//        public void TestSet()
//        {
//            MemoryPoolTest.TestMemoryLeak();
//            ContinuousQueue<int> queue = new ContinuousQueue<int>();

//            for (int x = 0; x < 1000; x++)
//            {
//                queue.Enqueue(x);
//            }

//            for (int x = 0; x < 500; x++)
//            {
//                queue.Dequeue();
//            }

//            for (int x = 1000; x < 1400; x++)
//            {
//                queue.Enqueue(x);
//            }

//            for (long x = queue.TailIndex; x <= queue.HeadIndex; x++)
//            {
//                queue[x] = (int)-x;
//                Assert.AreEqual((int)-x, queue[x]);
//            }
//            MemoryPoolTest.TestMemoryLeak();
//        }

//        [Test]
//        public void TestStack()
//        {
//            MemoryPoolTest.TestMemoryLeak();
//            Stack<int> baseLine = new Stack<int>();
//            ContinuousQueue<int> queue = new ContinuousQueue<int>();

//            for (int x = 1; x < 10000; x++)
//            {
//                Assert.IsTrue(baseLine.Reverse().SequenceEqual(queue));

//                if (x % 100 == 0)
//                {
//                    Assert.AreEqual(baseLine.Pop(), queue.Pop());
//                }

//                queue.Push(x);
//                baseLine.Push(x);

//                Assert.AreEqual(0, queue.TailIndex);
//                Assert.AreEqual(x - 1 - x / 100, queue.HeadIndex);
//                Assert.AreEqual(baseLine.Count, queue.Count);
//            }

//            while (queue.Count > 0)
//            {
//                Assert.AreEqual(baseLine.Count, queue.Count);
//                Assert.AreEqual(baseLine.Pop(), queue.Pop());
//                Assert.IsTrue(baseLine.Reverse().SequenceEqual(queue));
//            }
//            MemoryPoolTest.TestMemoryLeak();
//        }

//        [Test]
//        public void TestReverseStack()
//        {
//            MemoryPoolTest.TestMemoryLeak();
//            Stack<int> baseLine = new Stack<int>();
//            ContinuousQueue<int> queue = new ContinuousQueue<int>();

//            for (int x = 1; x < 10000; x++)
//            {
//                Assert.IsTrue(baseLine.SequenceEqual(queue));

//                if (x % 100 == 0)
//                {
//                    Assert.AreEqual(baseLine.Pop(), queue.RemoveFromTail());
//                }

//                queue.AddToTail(x);
//                baseLine.Push(x);

//                Assert.AreEqual(-(x - 1 - x / 100) - 1, queue.TailIndex);
//                Assert.AreEqual(-1, queue.HeadIndex);
//                Assert.AreEqual(baseLine.Count, queue.Count);
//            }

//            while (queue.Count > 0)
//            {
//                Assert.AreEqual(baseLine.Count, queue.Count);
//                Assert.AreEqual(baseLine.Pop(), queue.RemoveFromTail());
//                Assert.IsTrue(baseLine.SequenceEqual(queue));
//            }
//            MemoryPoolTest.TestMemoryLeak();
//        }
//    }
//}