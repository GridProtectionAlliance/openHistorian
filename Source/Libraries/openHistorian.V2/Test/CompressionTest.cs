using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace openHistorian.V2
{
    class CompressionTest
    {
        public static void Test()
        {
            TestMethod();
            TestMethod2();
            TestRandomGenerated();
            //TestByte(127);
            //TestByte(127 * 127);
            //TestByte(127 * 127 * 127);
            //TestByte(127 * 127 * 127 * 127);
            //TestByte(int.MaxValue);
            //TestByteRead(127);
            //TestByteRead(127 * 127);
            //TestByteRead(127 * 127 * 127);
            //TestByteRead(127 * 127 * 127 * 127);
            //TestByteRead(int.MaxValue);
            //TestRaw(int.MaxValue);
        }

        static unsafe void TestRandomGenerated()
        {
            Random rand = new Random();
            int seed = rand.Next();
            rand = new Random(seed);

            byte[] buffer = new byte[10];

            byte[] data = new byte[4];
            fixed (byte* lp = data)
            {
                for (int x = 0; x < 100000; x++)
                {
                    uint value, result;
                    int position = 1;

                    rand.NextBytes(data);
                    data[1] = 0;
                    data[2] = 0;
                    data[3] = 0;
                    value = *(uint*)lp;

                    Compression.Write7Bit(buffer, ref position, value);
                    if (position != 1 + Compression.Get7BitSize(value)) throw new Exception();
                    position = 1;
                    Compression.Read7BitUInt32(buffer, ref position, out result);
                    if (position != 1 + Compression.Get7BitSize(value)) throw new Exception();
                    if (result != value) throw new Exception();
                    position = 1;
                }

                for (int x = 0; x < 100000; x++)
                {
                    uint value, result;
                    int position = 1;

                    rand.NextBytes(data);
                    data[2] = 0;
                    data[3] = 0;
                    value = *(uint*)lp;

                    Compression.Write7Bit(buffer, ref position, value);
                    if (position != 1 + Compression.Get7BitSize(value)) throw new Exception();
                    position = 1;
                    Compression.Read7BitUInt32(buffer, ref position, out result);
                    if (position != 1 + Compression.Get7BitSize(value)) throw new Exception();
                    if (result != value) throw new Exception();
                    position = 1;
                }

                for (int x = 0; x < 100000; x++)
                {
                    uint value, result;
                    int position = 1;

                    rand.NextBytes(data);
                    data[3] = 0;
                    value = *(uint*)lp;

                    Compression.Write7Bit(buffer, ref position, value);
                    if (position != 1 + Compression.Get7BitSize(value)) throw new Exception();
                    position = 1;
                    Compression.Read7BitUInt32(buffer, ref position, out result);
                    if (position != 1 + Compression.Get7BitSize(value)) throw new Exception();
                    if (result != value) throw new Exception();
                    position = 1;
                }

                for (int x = 0; x < 100000; x++)
                {
                    uint value, result;
                    int position = 1;

                    rand.NextBytes(data);
                    value = *(uint*)lp;

                    Compression.Write7Bit(buffer, ref position, value);
                    if (position != 1 + Compression.Get7BitSize(value)) throw new Exception();
                    position = 1;
                    Compression.Read7BitUInt32(buffer, ref position, out result);
                    if (position != 1 + Compression.Get7BitSize(value)) throw new Exception();
                    if (result != value) throw new Exception();
                    position = 1;
                }
            }
        }

        static void TestMethod()
        {
            byte[] buffer = new byte[10];

            uint value, result;
            int position = 1;

            value = 127;
            Compression.Write7Bit(buffer, ref position, value);
            if (position != 2) throw new Exception();
            position = 1;
            Compression.Read7BitUInt32(buffer, ref position, out result);
            if (position != 2) throw new Exception();
            if (result != value) throw new Exception();
            position = 1;

            value = 127 * 127;
            Compression.Write7Bit(buffer, ref position, value);
            if (position != 3) throw new Exception();
            position = 1;
            Compression.Read7BitUInt32(buffer, ref position, out result);
            if (position != 3) throw new Exception();
            if (result != value) throw new Exception();
            position = 1;

            value = 127 * 127 * 127;
            Compression.Write7Bit(buffer, ref position, value);
            if (position != 4) throw new Exception();
            position = 1;
            Compression.Read7BitUInt32(buffer, ref position, out result);
            if (position != 4) throw new Exception();
            if (result != value) throw new Exception();
            position = 1;

            value = 127 * 127 * 127 * 127;
            Compression.Write7Bit(buffer, ref position, value);
            if (position != 5) throw new Exception();
            position = 1;
            Compression.Read7BitUInt32(buffer, ref position, out result);
            if (position != 5) throw new Exception();
            if (result != value) throw new Exception();
            position = 1;

            value = uint.MaxValue;
            Compression.Write7Bit(buffer, ref position, value);
            if (position != 6) throw new Exception();
            position = 1;
            Compression.Read7BitUInt32(buffer, ref position, out result);
            if (position != 6) throw new Exception();
            if (result != value) throw new Exception();
        }

        static void TestMethod2()
        {
            byte[] buffer = new byte[20];

            ulong value, result;
            int position = 1;

            value = 127;
            Compression.Write7Bit(buffer, ref position, value);
            if (position != 2) throw new Exception();
            position = 1;
            Compression.Read7BitUInt64(buffer, ref position, out result);
            if (position != 2) throw new Exception();
            if (result != value) throw new Exception();
            position = 1;

            value = 127 * 127;
            Compression.Write7Bit(buffer, ref position, value);
            if (position != 3) throw new Exception();
            position = 1;
            Compression.Read7BitUInt64(buffer, ref position, out result);
            if (position != 3) throw new Exception();
            if (result != value) throw new Exception();
            position = 1;

            value = 127 * 127 * 127;
            Compression.Write7Bit(buffer, ref position, value);
            if (position != 4) throw new Exception();
            position = 1;
            Compression.Read7BitUInt64(buffer, ref position, out result);
            if (position != 4) throw new Exception();
            if (result != value) throw new Exception();
            position = 1;

            value = 127 * 127 * 127 * 127;
            Compression.Write7Bit(buffer, ref position, value);
            if (position != 5) throw new Exception();
            position = 1;
            Compression.Read7BitUInt64(buffer, ref position, out result);
            if (position != 5) throw new Exception();
            if (result != value) throw new Exception();
            position = 1;

            value = 127L * 127 * 127 * 127 * 127;
            Compression.Write7Bit(buffer, ref position, value);
            if (position != 6) throw new Exception();
            position = 1;
            Compression.Read7BitUInt64(buffer, ref position, out result);
            if (position != 6) throw new Exception();
            if (result != value) throw new Exception();
            position = 1;

            value = 127L * 127 * 127 * 127 * 127 * 127;
            Compression.Write7Bit(buffer, ref position, value);
            if (position != 7) throw new Exception();
            position = 1;
            Compression.Read7BitUInt64(buffer, ref position, out result);
            if (position != 7) throw new Exception();
            if (result != value) throw new Exception();
            position = 1;

            value = 127L * 127 * 127 * 127 * 127 * 127 * 127;
            Compression.Write7Bit(buffer, ref position, value);
            if (position != 8) throw new Exception();
            position = 1;
            Compression.Read7BitUInt64(buffer, ref position, out result);
            if (position != 8) throw new Exception();
            if (result != value) throw new Exception();
            position = 1;

            value = 127L * 127 * 127 * 127 * 127 * 127 * 127 * 127;
            Compression.Write7Bit(buffer, ref position, value);
            if (position != 9) throw new Exception();
            position = 1;
            Compression.Read7BitUInt64(buffer, ref position, out result);
            if (position != 9) throw new Exception();
            if (result != value) throw new Exception();
            position = 1;

            value = 127L * 127 * 127 * 127 * 127 * 127 * 127 * 127 * 127;
            Compression.Write7Bit(buffer, ref position, value);
            if (position != 10) throw new Exception();
            position = 1;
            Compression.Read7BitUInt64(buffer, ref position, out result);
            if (position != 10) throw new Exception();
            if (result != value) throw new Exception();
            position = 1;

            value = ulong.MaxValue;
            Compression.Write7Bit(buffer, ref position, value);
            if (position != 10) throw new Exception();
            position = 1;
            Compression.Read7BitUInt64(buffer, ref position, out result);
            if (position != 10) throw new Exception();
            if (result != value) throw new Exception();
            position = 1;
        }


        static unsafe void TestRaw(int delta)
        {
            int loop = 10000;
            byte[] buffer = new byte[65535];
            int position = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int repeat = 0; repeat < loop; repeat++)
            {
                position = 0;
                int date = 216532156;
                int next;
                for (int x = 0; x < 1000; x++)
                {
                    next = date + delta;
                    fixed (byte* lp = buffer)
                    {
                        *(int*)(lp + position) = next;
                    }
                    position += 4;
                    next = date;
                }
            }
            sw.Stop();
            MessageBox.Show(position.ToString() + "b " + ((sw.Elapsed.TotalMilliseconds * 1000) / loop).ToString());
            //MessageBox.Show(position.ToString() + "b " + ((position / sw.Elapsed.TotalSeconds) * loop / Math.Pow(2, 20)).ToString());
        }

        static void TestByte(uint delta)
        {
            int loop = 10000;
            byte[] buffer = new byte[65535];
            int position = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int repeat = 0; repeat < loop; repeat++)
            {
                position = 0;
                for (int x = 0; x < 200; x++)
                {
                    Compression.Write7Bit(buffer, ref position, delta);
                    Compression.Write7Bit(buffer, ref position, delta);
                    Compression.Write7Bit(buffer, ref position, delta);
                    Compression.Write7Bit(buffer, ref position, delta);
                    Compression.Write7Bit(buffer, ref position, delta);

                    //Compression.Write(buffer, ref position, delta, delta, delta, delta);
                    //Compression.Write(buffer, ref position, delta, delta, delta, delta);
                    //Compression.Write(buffer, ref position, delta, delta, delta, delta);
                    //Compression.Write(buffer, ref position, delta, delta, delta, delta);
                    //Compression.Write(buffer, ref position, delta, delta, delta, delta);

                }
            }
            sw.Stop();
            MessageBox.Show(position.ToString() + "b " + (loop * 1000 / sw.Elapsed.TotalSeconds / 1000000).ToString());
        }
        static void TestByteRead(uint delta)
        {
            int loop = 10000;
            byte[] buffer = new byte[65535];
            int position = 0;

            for (int x = 0; x < 200; x++)
            {
                Compression.Write7Bit(buffer, ref position, delta);
                Compression.Write7Bit(buffer, ref position, delta);
                Compression.Write7Bit(buffer, ref position, delta);
                Compression.Write7Bit(buffer, ref position, delta);
                Compression.Write7Bit(buffer, ref position, delta);
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int repeat = 0; repeat < loop; repeat++)
            {
                position = 0;
                for (int x = 0; x < 200; x++)
                {
                    Compression.Read7BitUInt32(buffer, ref position, out delta);
                    Compression.Read7BitUInt32(buffer, ref position, out delta);
                    Compression.Read7BitUInt32(buffer, ref position, out delta);
                    Compression.Read7BitUInt32(buffer, ref position, out delta);
                    Compression.Read7BitUInt32(buffer, ref position, out delta);
                }
            }
            sw.Stop();
            MessageBox.Show(position.ToString() + "b " + (loop * 1000 / sw.Elapsed.TotalSeconds / 1000000).ToString());
        }

        //static unsafe void TestRawRead(int delta)
        //{
        //    int loop = 10000;
        //    byte[] buffer = new byte[65535];
        //    int position = 0;

        //    for (int repeat = 0; repeat < 1; repeat++)
        //    {
        //        position = 0;
        //        int date = 216532156;
        //        int next;
        //        for (int x = 0; x < 1000; x++)
        //        {
        //            next = date + delta;

        //            fixed (byte* lp = buffer)
        //            {
        //                *(int*)(lp + position) = next;
        //            }
        //            position += 4;
        //            next = date;
        //        }
        //    }


        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();
        //    for (int repeat = 0; repeat < loop; repeat++)
        //    {
        //        position = 0;
        //        int date = 216532156;
        //        int next;
        //        for (int x = 0; x < 1000; x++)
        //        {
        //            next = date + delta;

        //            fixed (byte* lp = buffer)
        //            {
        //                next = *(int*)(lp + position);
        //            }
        //            position += 4;
        //            next = date;
        //        }
        //    }
        //    sw.Stop();
        //    MessageBox.Show(position.ToString() + "b " + ((position / sw.Elapsed.TotalSeconds) * loop / Math.Pow(2, 20)).ToString());
        //}
        //static void TestByteRead(int delta)
        //{
        //    int loop = 10000;
        //    byte[] buffer = new byte[65535];
        //    int position = 0;

        //    for (int repeat = 0; repeat < 1; repeat++)
        //    {
        //        position = 0;
        //        int next = 216532156;
        //        for (int x = 0; x < 1000; x++)
        //        {
        //            position = Compression.Compress(buffer, position, next, next + delta);
        //            next += delta;
        //        }
        //    }

        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();
        //    for (int repeat = 0; repeat < loop; repeat++)
        //    {
        //        position = 0;
        //        int next = 216532156;
        //        for (int x = 0; x < 1000; x++)
        //        {
        //            next = Compression.Decompress(buffer, position, next, out position);
        //        }
        //    }
        //    sw.Stop();
        //    MessageBox.Show(position.ToString() + "b " + ((position / sw.Elapsed.TotalSeconds) * loop / Math.Pow(2, 20)).ToString());
        //}
    }
}
