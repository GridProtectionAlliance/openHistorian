using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace openHistorian.Engine.ArchiveWriters
{
    [TestFixture]
    public class PointStreamCacheTest
    {
        [Test]
        public void Test1()
        {
            const int count = 10000;
            using (var cache = new PointStreamCache())
            {
                Assert.IsFalse(cache.IsReading);

                var stream1 = new PointStreamSequential(10, count);

                ulong key1, key2, value1, value2;

                for (uint x = 0; x < 10; x++)
                {
                    Assert.AreEqual((int)x, cache.Count);
                    stream1.Read(out key1, out key2, out value1, out value2);
                    cache.Write(key1, key2, value1, value2);
                }
                cache.Write(stream1);
                
                Assert.AreEqual(count, cache.Count);
                Assert.IsFalse(cache.IsReading);

                cache.SetReadingFromBeginning();

                Assert.AreEqual(count, cache.Count);
                Assert.IsTrue(cache.IsReading);

                Assert.IsTrue(cache.AreEqual(new PointStreamSequential(10, count)));
                Assert.IsFalse(cache.AreEqual(new PointStreamSequential(10, count)));
                cache.SetReadingFromBeginning();
                Assert.IsTrue(cache.AreEqual(new PointStreamSequential(10, count)));

                cache.ClearAndSetWriting();
                Assert.AreEqual(cache.Count,0);

                stream1 = new PointStreamSequential(100, count-1000);
                cache.Write(stream1);
                cache.SetReadingFromBeginning();
                Assert.IsTrue(cache.AreEqual(new PointStreamSequential(100, count - 1000)));
            }


        }

    }
}
