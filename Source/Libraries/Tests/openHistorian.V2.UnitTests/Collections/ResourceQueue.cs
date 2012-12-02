using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using openHistorian.V2.Collections;

namespace openHistorian.V2.Collections.Test
{
    [TestFixture()]
    public class ResourceQueueTest
    {
        [Test()]
        public void Test()
        {
            int x = 0;
            ResourceQueue<string> queue = new ResourceQueue<string>(() => (x++).ToString(),3,4);

            x = 10;

            Assert.AreEqual(queue.Dequeue(), "0");
            Assert.AreEqual(queue.Dequeue(), "1");
            Assert.AreEqual(queue.Dequeue(), "2");
            Assert.AreEqual(queue.Dequeue(), "10");
            Assert.AreEqual(queue.Dequeue(), "11");

            queue.Enqueue("0");
            queue.Enqueue("1");
            Assert.AreEqual(queue.Dequeue(), "0");
            queue.Enqueue("3");
            Assert.AreEqual(queue.Dequeue(), "1");
            Assert.AreEqual(queue.Dequeue(), "3");
            Assert.AreEqual(queue.Dequeue(), "12");

            queue.Enqueue("1");
            queue.Enqueue("2");
            queue.Enqueue("3");
            queue.Enqueue("4");
            queue.Enqueue("5");
            Assert.AreEqual(queue.Dequeue(), "1");
            Assert.AreEqual(queue.Dequeue(), "2");
            Assert.AreEqual(queue.Dequeue(), "3");
            Assert.AreEqual(queue.Dequeue(), "4");
            Assert.AreEqual(queue.Dequeue(), "13");
        }

    }
}
