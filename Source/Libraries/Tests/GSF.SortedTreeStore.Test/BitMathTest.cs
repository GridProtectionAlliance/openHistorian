using System;
using NUnit.Framework;

namespace GSF.Test
{
    [TestFixture()]
    public class BitMathTest
    {
        [Test()]
        public void CountBitsSet()
        {
            Assert.AreEqual(0, BitMath.CountBitsSet(0));
            Assert.AreEqual(32, BitMath.CountBitsSet(uint.MaxValue));
            Assert.AreEqual(31, BitMath.CountBitsSet(int.MaxValue));
            Assert.AreEqual(1, BitMath.CountBitsSet(short.MaxValue + 1));
            Assert.AreEqual(10, BitMath.CountBitsSet(0xF030207));


            Assert.AreEqual(0, BitMath.CountBitsSet((ulong)0));
            Assert.AreEqual(32, BitMath.CountBitsSet((ulong)uint.MaxValue));
            Assert.AreEqual(31, BitMath.CountBitsSet((ulong)int.MaxValue));
            Assert.AreEqual(1, BitMath.CountBitsSet((ulong)short.MaxValue + 1));
            Assert.AreEqual(10, BitMath.CountBitsSet((ulong)0xF030207));
            int x = -1;
            Assert.AreEqual(64, BitMath.CountBitsSet(ulong.MaxValue));
            Assert.AreEqual(64, BitMath.CountBitsSet((ulong)x));
        }

        [Test()]
        public void CountBitsCleared()
        {
            Assert.AreEqual(32 - 0, BitMath.CountBitsCleared(0));
            Assert.AreEqual(32 - 32, BitMath.CountBitsCleared(uint.MaxValue));
            Assert.AreEqual(32 - 31, BitMath.CountBitsCleared(int.MaxValue));
            Assert.AreEqual(32 - 1, BitMath.CountBitsCleared(short.MaxValue + 1));
            Assert.AreEqual(32 - 10, BitMath.CountBitsCleared(0xF030207));


            Assert.AreEqual(64 - 0, BitMath.CountBitsCleared((ulong)0));
            Assert.AreEqual(64 - 32, BitMath.CountBitsCleared((ulong)uint.MaxValue));
            Assert.AreEqual(64 - 31, BitMath.CountBitsCleared((ulong)int.MaxValue));
            Assert.AreEqual(64 - 1, BitMath.CountBitsCleared((ulong)short.MaxValue + 1));
            Assert.AreEqual(64 - 10, BitMath.CountBitsCleared((ulong)0xF030207));
            int x = -1;
            Assert.AreEqual(64 - 64, BitMath.CountBitsCleared(ulong.MaxValue));
            Assert.AreEqual(64 - 64, BitMath.CountBitsCleared((ulong)x));
        }

        [Test()]
        public void IsPowerOfTwo()
        {
            Assert.AreEqual(false, BitMath.IsPowerOfTwo((uint)0));
            Assert.AreEqual(true, BitMath.IsPowerOfTwo((uint)1));
            Assert.AreEqual(false, BitMath.IsPowerOfTwo((uint)3));
            Assert.AreEqual(true, BitMath.IsPowerOfTwo((uint)1 << 31));

            Assert.AreEqual(false, BitMath.IsPowerOfTwo((ulong)0));
            Assert.AreEqual(true, BitMath.IsPowerOfTwo((ulong)1));
            Assert.AreEqual(false, BitMath.IsPowerOfTwo((ulong)3));
            Assert.AreEqual(true, BitMath.IsPowerOfTwo((ulong)1 << 63));
        }

        [Test()]
        public void CountLeadinZeros()
        {
            Random r = new Random();

            Assert.AreEqual(32, BitMath.CountLeadingZeros(0u));
            Assert.AreEqual(0, BitMath.CountLeadingZeros(uint.MaxValue));

            for (int k = 0; k < 10; k++)
            {
                for (int x = 0; x < 32; x++)
                {
                    Assert.AreEqual(31 - x, BitMath.CountLeadingZeros(1u << x | Next(r, 1u << x)));
                }
            }

            Assert.AreEqual(64, BitMath.CountLeadingZeros(0ul));
            Assert.AreEqual(0, BitMath.CountLeadingZeros(ulong.MaxValue));

            for (int k = 0; k < 10; k++)
            {
                for (int x = 0; x < 63; x++)
                {
                    Assert.AreEqual(63 - x, BitMath.CountLeadingZeros(1ul << x | Next(r, 1ul << x)));
                }
            }
        }

        private static ulong Next(Random r, ulong maxValue)
        {
            return ((ulong)r.Next() << 32 | (uint)r.Next()) % maxValue;
        }

        private static uint Next(Random r, uint maxValue)
        {
            return (uint)r.Next() % maxValue;
        }

        [Test()]
        public void CountTrailingZeros()
        {
            Random r = new Random();

            Assert.AreEqual(32, BitMath.CountTrailingZeros(0u));
            Assert.AreEqual(0, BitMath.CountTrailingZeros(uint.MaxValue));

            for (int k = 0; k < 10; k++)
            {
                for (int x = 0; x < 32; x++)
                {
                    Assert.AreEqual(x, BitMath.CountTrailingZeros(1u << x | NextAbove(r, 1u << x)));
                }
            }

            Assert.AreEqual(64, BitMath.CountTrailingZeros(0ul));
            Assert.AreEqual(0, BitMath.CountTrailingZeros(ulong.MaxValue));

            for (int k = 0; k < 10; k++)
            {
                for (int x = 0; x < 63; x++)
                {
                    Assert.AreEqual(x, BitMath.CountTrailingZeros(1ul << x | NextAbove(r, 1ul << x)));
                }
            }
        }

        private static ulong NextAbove(Random r, ulong maxValue)
        {
            return ((ulong)r.Next() << 32 | (uint)r.Next()) & ~(maxValue - 1);
        }

        private static uint NextAbove(Random r, uint maxValue)
        {
            return (uint)r.Next() & ~(maxValue - 1);
        }


        [Test()]
        public void RoundUpToNearestPowerOfTwo()
        {
            Assert.AreEqual(1u, BitMath.RoundUpToNearestPowerOfTwo(0));
            Assert.AreEqual(1u, BitMath.RoundUpToNearestPowerOfTwo(1));
            Assert.AreEqual(4u, BitMath.RoundUpToNearestPowerOfTwo(3));
            Assert.AreEqual(1u << 27, BitMath.RoundUpToNearestPowerOfTwo((uint)(1 << 27) - 256));
            Assert.AreEqual(1u << 30, BitMath.RoundUpToNearestPowerOfTwo((1u << 30) - 1));
            Assert.AreEqual(1u << 31, BitMath.RoundUpToNearestPowerOfTwo(uint.MaxValue));

            Assert.AreEqual(1ul, BitMath.RoundUpToNearestPowerOfTwo((ulong)0));
            Assert.AreEqual(1ul, BitMath.RoundUpToNearestPowerOfTwo((ulong)1));
            Assert.AreEqual(4ul, BitMath.RoundUpToNearestPowerOfTwo((ulong)3));
            Assert.AreEqual(1ul << 27, BitMath.RoundUpToNearestPowerOfTwo(((ulong)1 << 27) - 256));
            Assert.AreEqual(1ul << 62, BitMath.RoundUpToNearestPowerOfTwo((1ul << 62) - 1));
            Assert.AreEqual(1ul << 63, BitMath.RoundUpToNearestPowerOfTwo((ulong)1 << 63));
            Assert.AreEqual(1ul << 63, BitMath.RoundUpToNearestPowerOfTwo(ulong.MaxValue));
        }

        [Test()]
        public void RoundDownToNearestPowerOfTwo()
        {
            Assert.AreEqual(1u, BitMath.RoundDownToNearestPowerOfTwo(0u));
            Assert.AreEqual(1u, BitMath.RoundDownToNearestPowerOfTwo(1u));
            Assert.AreEqual(2u, BitMath.RoundDownToNearestPowerOfTwo(3u));
            Assert.AreEqual(1u << 26, BitMath.RoundDownToNearestPowerOfTwo((1u << 27) - 256));
            Assert.AreEqual(1u << 29, BitMath.RoundDownToNearestPowerOfTwo((1u << 30) - 1));
            Assert.AreEqual(1u << 30, BitMath.RoundDownToNearestPowerOfTwo(1u << 30));
            Assert.AreEqual(1u << 31, BitMath.RoundDownToNearestPowerOfTwo(uint.MaxValue));

            Assert.AreEqual(1ul, BitMath.RoundDownToNearestPowerOfTwo(0ul));
            Assert.AreEqual(1ul, BitMath.RoundDownToNearestPowerOfTwo(1ul));
            Assert.AreEqual(2ul, BitMath.RoundDownToNearestPowerOfTwo(3ul));
            Assert.AreEqual(1ul << 26, BitMath.RoundDownToNearestPowerOfTwo((1ul << 27) - 256));
            Assert.AreEqual(1u << 30, BitMath.RoundDownToNearestPowerOfTwo(1ul << 30));
            Assert.AreEqual(1ul << 29, BitMath.RoundDownToNearestPowerOfTwo((1ul << 30) - 1));
            Assert.AreEqual(1ul << 63, BitMath.RoundDownToNearestPowerOfTwo(ulong.MaxValue));
        }

        [Test()]
        public void CreateBitMask()
        {
            for (int x = 0; x < 64; x++)
            {
                Assert.AreEqual((1ul << x) - 1, BitMath.CreateBitMask(x));
            }
            Assert.AreEqual(ulong.MaxValue, BitMath.CreateBitMask(64));
        }
    }
}