using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using openHistorian.FileStructure.IO;

namespace openHistorian.FileStructure
{
    [TestFixture]
    internal class Murmur3Test
    {
        //[Test]
        //public unsafe void TestIsSame()
        //{
        //    byte[] data = new byte[4096];
        //    Random r = new Random();

        //    for (int x = 0; x < 100; x++)
        //    {
        //        r.NextBytes(data);

        //        Murmur3Orig mm3 = new Murmur3Orig();
        //        byte[] checksum = mm3.ComputeHash(data);
        //        byte[] checksum2 = new byte[16];

        //        fixed (byte* lp = data)
        //        {
        //            ulong value1;
        //            ulong value2;
        //            Murmur3.ComputeHash(lp, data.Length, out value1, out value2);

        //            Array.Copy(BitConverter.GetBytes(value1), 0, checksum2, 0, 8);
        //            Array.Copy(BitConverter.GetBytes(value2), 0, checksum2, 8, 8);
        //        }

        //        Assert.IsTrue(checksum2.SequenceEqual(checksum));
        //    }
        //}

        //[Test]
        //public unsafe void Benchmark()
        //{
        //    byte[] data = new byte[4096];
        //    Random r = new Random(1);
        //    r.NextBytes(data);

        //    //Prime the run
        //    Murmur3Orig mm3 = new Murmur3Orig();
        //    for (int x = 0; x < 1000; x++)
        //        mm3.ComputeHash(data);
        //    fixed (byte* lp = data)
        //    {
        //        ulong value1;
        //        ulong value2;
        //        long value3;
        //        int value4;
        //        for (int x = 0; x < 1000; x++)
        //            Murmur3.ComputeHash(lp, data.Length, out value1, out value2);

        //        for (int x = 0; x < 1000; x++)
        //            Footer.ComputeChecksum((IntPtr)lp, out value3, out value4, data.Length);
        //    }


        //    Stopwatch sw1 = new Stopwatch();
        //    Stopwatch sw2 = new Stopwatch();
        //    Stopwatch sw3 = new Stopwatch();

        //    sw1.Start();
        //    for (int x = 0; x < 10000; x++)
        //        mm3.ComputeHash(data);
        //    sw1.Stop();

        //    fixed (byte* lp = data)
        //    {
        //        ulong value1;
        //        ulong value2;
        //        long value3;
        //        int value4;
        //        sw2.Start();
        //        for (int x = 0; x < 10000; x++)
        //            Murmur3.ComputeHash(lp, data.Length, out value1, out value2);
        //        sw2.Stop();

        //        sw3.Start();
        //        for (int x = 0; x < 10000; x++)
        //            Footer.ComputeChecksum((IntPtr)lp, out value3, out value4, data.Length);
        //        sw3.Stop();
        //    }

        //    Console.WriteLine("orig: " + (4096 * 10000 / sw1.Elapsed.TotalSeconds / 1024 / 1024).ToString("0 MB/S"));
        //    Console.WriteLine("mine: " + (4096 * 10000 / sw2.Elapsed.TotalSeconds / 1024 / 1024).ToString("0 MB/S"));
        //    Console.WriteLine("old: " + (4096 * 10000 / sw3.Elapsed.TotalSeconds / 1024 / 1024).ToString("0 MB/S"));
        //}
    }
}