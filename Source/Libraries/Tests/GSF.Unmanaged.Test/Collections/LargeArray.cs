using System;
using GSF;
using NUnit.Framework;
using GSF.Collections;

namespace openHistorian.Collections.Test
{
    [TestFixture]
    public class LargeArray
    {
        [Test]
        public void TestLargeArray()
        {
            TestArray(new LargeArray<int>());
        }

        [Test]
        public unsafe void TestLargeUnmanagedArray()
        {
            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0);
            using (var array = new LargeUnmanagedArray<int>(4, Globals.BufferPool, ptr => *(int*)ptr, (ptr, v) => *(int*)ptr = v))
            {
                TestArray(array);
            }
            Assert.AreEqual(Globals.BufferPool.AllocatedBytes, 0);

        }

        public void TestArray(ILargeArray<int> array)
        {
            for (int x = 0; x < 2500000; x++)
            {
                if (x >= array.Capacity)
                {
                    HelperFunctions.ExpectError(() => array[x] = x);
                    array.SetCapacity(array.Capacity + 1);
                }
                array[x] = x;
            }

            for (int x = 0; x < 2500000; x++)
            {
                Assert.AreEqual(array[x], x);
            }
        }
    }
}
