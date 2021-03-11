using System;
using GSF.IO.Unmanaged;
using NUnit.Framework;

namespace openHistorian.PerformanceTests.IO
{
    [TestFixture]
    unsafe public class BinaryStreamBenchmark
    {
        [Test]
        public void Test7Bit1()
        {
            byte[] data = new byte[4096*5];
            fixed (byte* lp = data)
            {
                using (BinaryStreamPointerWrapper bs = new BinaryStreamPointerWrapper(lp, data.Length))
                {
                    DebugStopwatch sw = new DebugStopwatch();
                    double time = sw.TimeEventMedian(() =>
                    {
                        for (int repeat = 0; repeat < 1000; repeat++)
                        {
                            bs.Position = 0;
                            for (int x = 0; x < 1000; x++)
                            {
                                bs.Write7Bit(1u);
                                bs.Write7Bit(1u);
                                bs.Write7Bit(1u);
                                bs.Write7Bit(1u);
                            }
                        }

                    });
                    Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
                }
            }
        }
        [Test]
        public void Test7Bit2()
        {
            byte[] data = new byte[4096 * 5];
            fixed (byte* lp = data)
            {
                using (BinaryStreamPointerWrapper bs = new BinaryStreamPointerWrapper(lp, data.Length))
                {
                    DebugStopwatch sw = new DebugStopwatch();
                    double time = sw.TimeEventMedian(() =>
                    {
                        for (int repeat = 0; repeat < 1000; repeat++)
                        {
                            bs.Position = 0;
                            for (int x = 0; x < 1000; x++)
                            {
                                bs.Write7Bit(128u);
                                bs.Write7Bit(128u);
                                bs.Write7Bit(128u);
                                bs.Write7Bit(128u);
                            }
                        }

                    });
                    Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
                }
            }
        }

        [Test]
        public void Test7Bit3()
        {
            byte[] data = new byte[4096 * 5];
            fixed (byte* lp = data)
            {
                using (BinaryStreamPointerWrapper bs = new BinaryStreamPointerWrapper(lp, data.Length))
                {
                    DebugStopwatch sw = new DebugStopwatch();
                    double time = sw.TimeEventMedian(() =>
                    {
                        for (int repeat = 0; repeat < 1000; repeat++)
                        {
                            bs.Position = 0;
                            for (int x = 0; x < 1000; x++)
                            {
                                bs.Write7Bit(128u * 128u);
                                bs.Write7Bit(128u * 128u);
                                bs.Write7Bit(128u * 128u);
                                bs.Write7Bit(128u * 128u);
                            }
                        }

                    });
                    Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
                }
            }
        }

        [Test]
        public void Test7Bit4()
        {
            byte[] data = new byte[4096 * 5];
            fixed (byte* lp = data)
            {
                using (BinaryStreamPointerWrapper bs = new BinaryStreamPointerWrapper(lp, data.Length))
                {
                    DebugStopwatch sw = new DebugStopwatch();
                    double time = sw.TimeEventMedian(() =>
                    {
                        for (int repeat = 0; repeat < 1000; repeat++)
                        {
                            bs.Position = 0;
                            for (int x = 0; x < 1000; x++)
                            {
                                bs.Write7Bit(128u * 128u * 128u);
                                bs.Write7Bit(128u * 128u * 128u);
                                bs.Write7Bit(128u * 128u * 128u);
                                bs.Write7Bit(128u * 128u * 128u);
                            }
                        }

                    });
                    Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
                }
            }
        }

        [Test]
        public void Test7Bit5()
        {
            byte[] data = new byte[4096 * 6];
            fixed (byte* lp = data)
            {
                using (BinaryStreamPointerWrapper bs = new BinaryStreamPointerWrapper(lp, data.Length))
                {
                    DebugStopwatch sw = new DebugStopwatch();
                    double time = sw.TimeEventMedian(() =>
                    {
                        for (int repeat = 0; repeat < 1000; repeat++)
                        {
                            bs.Position = 0;
                            for (int x = 0; x < 1000; x++)
                            {
                                bs.Write7Bit(uint.MaxValue);
                                bs.Write7Bit(uint.MaxValue);
                                bs.Write7Bit(uint.MaxValue);
                                bs.Write7Bit(uint.MaxValue);
                            }
                        }

                    });
                    Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
                }
            }
        }


        [Test]
        public void TestWriteByte()
        {
            byte[] data = new byte[4096];
            fixed (byte* lp = data)
            {
                using (BinaryStreamPointerWrapper bs = new BinaryStreamPointerWrapper(lp, data.Length))
                {
                    DebugStopwatch sw = new DebugStopwatch();
                    double time = sw.TimeEventMedian(() =>
                    {
                        for (int repeat = 0; repeat < 1000; repeat++)
                        {
                            bs.Position = 0;
                            for (int x = 0; x < 1000; x++)
                            {
                                bs.Write((sbyte)x);
                                bs.Write((sbyte)x);
                                bs.Write((sbyte)x);
                                bs.Write((sbyte)x);
                            }
                        }

                    });

                    Console.WriteLine(4*1000*1000/time/1000/1000);

                }
            }
        }

        [Test]
        public void TestWriteShort()
        {
            byte[] data = new byte[4096*2];
            fixed (byte* lp = data)
            {
                using (BinaryStreamPointerWrapper bs = new BinaryStreamPointerWrapper(lp, data.Length))
                {
                    DebugStopwatch sw = new DebugStopwatch();
                    double time = sw.TimeEventMedian(() =>
                    {
                        for (int repeat = 0; repeat < 1000; repeat++)
                        {
                            bs.Position = 0;
                            for (int x = 0; x < 1000; x++)
                            {
                                bs.Write((short)x);
                                bs.Write((short)x);
                                bs.Write((short)x);
                                bs.Write((short)x);
                            }
                        }

                    });

                    Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);

                }
            }
        }

        [Test]
        public void TestWriteInt()
        {
            byte[] data = new byte[4096 * 4];
            fixed (byte* lp = data)
            {
                using (BinaryStreamPointerWrapper bs = new BinaryStreamPointerWrapper(lp, data.Length))
                {
                    DebugStopwatch sw = new DebugStopwatch();
                    double time = sw.TimeEventMedian(() =>
                    {
                        for (int repeat = 0; repeat < 1000; repeat++)
                        {
                            bs.Position = 0;
                            for (int x = 0; x < 1000; x++)
                            {
                                bs.Write(x);
                                bs.Write(x);
                                bs.Write(x);
                                bs.Write(x);
                            }
                        }

                    });

                    Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
                }
            }
        }

        [Test]
        public void TestWriteLong()
        {
            byte[] data = new byte[4096 * 8];
            fixed (byte* lp = data)
            {
                using (BinaryStreamPointerWrapper bs = new BinaryStreamPointerWrapper(lp, data.Length))
                {
                    DebugStopwatch sw = new DebugStopwatch();
                    double time = sw.TimeEventMedian(() =>
                    {
                        for (int repeat = 0; repeat < 1000; repeat++)
                        {
                            bs.Position = 0;
                            for (int x = 0; x < 1000; x++)
                            {
                                bs.Write((long)x);
                                bs.Write((long)x);
                                bs.Write((long)x);
                                bs.Write((long)x);
                            }
                        }

                    });

                    Console.WriteLine(4 * 1000 * 1000 / time / 1000 / 1000);
                }
            }
        }

    }
}
