using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace openHistorian.IO.Unmanaged
{
    [TestFixture]
    public class MemoryStreamTest
    {
        [Test]
        public void TestBlocksPerSecond()
        {
            UnmanagedMemory.Memory.UseLargePages = true;
            DebugStopwatch sw = new DebugStopwatch();
            using (MemoryStream ms = new MemoryStream())
            {
                using (var io = ms.GetNextIoSession())
                {
                    IntPtr ptr;
                    long pos;
                    int len;
                    bool write;
                    io.GetBlock(ms.BlockSize * 2000L - 1, true, out ptr, out pos, out len, out write);

                    double sec = sw.TimeEvent(() =>
                        {
                            for (int y = 0; y < 100; y++ )
                                for (int x = 0; x < 2000; x++)
                                    io.GetBlock((long)x * ms.BlockSize, true, out ptr, out pos, out len, out write);
                        });

                    Console.WriteLine("Get Blocks: " + (200000/sec/1000000).ToString("0.00 Million Per Second"));
                }
            }



        }
    }
}
