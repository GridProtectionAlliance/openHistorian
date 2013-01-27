using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace GSF.Threading
{
    [TestFixture]
    class TreadSafeListTest
    {
        [Test]
        public void Test()
        {
            var ts = new ThreadSafeList<int>();

            for (int x = 0; x < 10; x++)
            {
                ts.Add(x);
            }
            
            Assert.AreEqual(10, ts.Count());
            Assert.IsTrue(ts.Remove(5));
            Assert.AreEqual(9, ts.Count());
            Assert.IsFalse(ts.Remove(5));

            int count = 0;
            foreach (var x in ts)
            {
                count++;
                ts.ForEach((i) => ts.Remove(i));
            }
            Assert.AreEqual(1, count);

        }
    }
}
