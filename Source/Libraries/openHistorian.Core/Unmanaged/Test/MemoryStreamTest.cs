using System;

namespace openHistorian.Core.Unmanaged
{
    class MemoryStreamTest
    {
        public static void Test()
        {
            SelfTest();
            MemoryStream ms = new MemoryStream();
            openHistorian.Core.Unmanaged.BinaryStreamTest.Test(ms);
        }
        static void SelfTest()
        {
            MemoryStream ms = new MemoryStream();
            Random rand = new Random();
            int seed = rand.Next();
            rand = new Random(seed);
            byte[] data = new byte[255];
            rand.NextBytes(data);

            while (ms.Position < 1000000)
            {
                ms.Write(data, 0, rand.Next(256));
            }

            byte[] data2 = new byte[255];
            rand = new Random(seed);
            rand.NextBytes(data2);
            ms.Position = 0;
            Compare(data, data2, 255);
            while (ms.Position < 1000000)
            {
                int length = rand.Next(256);
                ms.Read(data2, 0, length);
                Compare(data, data2, length);
            }
        }
        static void Compare(byte[] a, byte[] b, int length)
        {
            for (int x = 0; x < length; x++)
            {
                if (a[x] != b[x])
                    throw new Exception();
            }
        }
    }
}
