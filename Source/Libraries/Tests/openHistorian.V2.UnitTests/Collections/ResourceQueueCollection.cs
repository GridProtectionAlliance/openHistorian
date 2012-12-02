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
    public class ResourceQueueCollectionTest
    {

        [Test()]
        public void Test()
        {
            ResourceQueueCollection<int, string> queue = new ResourceQueueCollection<int, string>((x) => () => x.ToString(), 3, 3);

            Assert.AreEqual("1", queue[1].Dequeue());
            Assert.AreEqual("250", queue[250].Dequeue());
            Assert.AreEqual("999", queue[999].Dequeue());

            queue[250].Enqueue("0");

            Assert.AreEqual("250", queue[250].Dequeue());
            Assert.AreEqual("250", queue[250].Dequeue());
            Assert.AreEqual("0", queue[250].Dequeue());
            Assert.AreEqual("250", queue[250].Dequeue());

        }

    }
}
