using System.Linq;
using NUnit.Framework;

namespace GSF.Threading.Test
{
    [TestFixture]
    internal class TreadSafeListTest
    {
        [Test]
        public void Test()
        {
            ThreadSafeList<int> ts = new ThreadSafeList<int>();

            for (int x = 0; x < 10; x++)
            {
                ts.Add(x);
            }

            Assert.AreEqual(10, ts.Count());
            Assert.IsTrue(ts.Remove(5));
            Assert.AreEqual(9, ts.Count());
            Assert.IsFalse(ts.Remove(5));

            int count = 0;
            foreach (int x in ts)
            {
                count++;
                ts.ForEach((i) => ts.Remove(i));
            }
            Assert.AreEqual(1, count);
        }
    }
}