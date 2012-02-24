//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using System.Diagnostics;
//using System.Windows.Forms;

//namespace Historian
//{
//    class CompressionTest
//    {
//        public static void Test()
//        {
//            //PinningCost();
//            //TestMS();
//            //TestByteRead(1);
//            //TestByteRead(127);
//            //TestByteRead(127 * 127);
//            //TestByteRead(127 * 127 * 127);
//            //TestByteRead(127 * 127 * 127 * 127);
//            //TestRawRead(127 * 127 * 127 * 127);
//            TestByte(127);
//            TestByte(127 * 127);
//            TestByte(127 * 127 * 127);
//            TestByte(127 * 127 * 127 * 127);
//            //TestRaw(127 * 127 * 127 * 127);
//            //TestByte(int.MaxValue);
//            TestCustomClass2(0xFF);
//            TestCustomClass2(0xFFFF);
//            TestCustomClass2(0xFFFFFF);
//            TestCustomClass2(0x7FFFFFF);
//            //TestCustomClass(1);
//            //TestCustomClass(127);
//            //TestCustomClass(127 * 127);
//            //TestCustomClass(127 * 127 * 127);
//            //TestCustomClass(127 * 127 * 127 * 127);
//            //TestCustomClassStatic(1);
//            //TestCustomClassStatic(127);
//            //TestCustomClassStatic(127 * 127);
//            //TestCustomClassStatic(127 * 127 * 127);
//            //TestCustomClassStatic(127 * 127 * 127 * 127);
//        }
//        static void TestMS()
//        {
//            int loop = 10000;
//            int length = 0;

//            MemoryStream ms = new System.IO.MemoryStream(65536);

//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//            for (int repeat = 0; repeat < loop; repeat++)
//            {
//                length = (int)ms.Position;
//                ms.Position = 0;
//                int date = 216532156;
//                int next;
//                for (int x = 0; x < 1000; x++)
//                {
//                    next = date + 289475345;
//                    Compression.Compress(ms, date, next);
//                    next = date;
//                }
//            }
//            sw.Stop();
//            MessageBox.Show(length.ToString() + "b " + ((length / sw.Elapsed.TotalSeconds) * loop / Math.Pow(2, 20)).ToString());

//        }
//        static unsafe void TestRaw(int delta)
//        {
//            int loop = 10000;
//            byte[] buffer = new byte[65535];
//            int position = 0;

//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//            for (int repeat = 0; repeat < loop; repeat++)
//            {
//                position = 0;
//                int date = 216532156;
//                int next;
//                for (int x = 0; x < 1000; x++)
//                {
//                    next = date + delta;

//                    fixed (byte* lp = buffer)
//                    {
//                        *(int*)(lp + position) = next;
//                    }
//                    position += 4;
//                    next = date;
//                }
//            }
//            sw.Stop();
//            MessageBox.Show(position.ToString() + "b " + ((position / sw.Elapsed.TotalSeconds) * loop / Math.Pow(2, 20)).ToString());
//        }
//        static void TestByte(uint delta)
//        {
//            int loop = 10000;
//            byte[] buffer = new byte[65535];
//            int position = 0;

//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//            for (int repeat = 0; repeat < loop; repeat++)
//            {
//                position = 0;
//                //int next = 216532156;
//                for (int x = 0; x < 50; x++)
//                {
//                    position = Compression.Write7Bit2(buffer, position, delta, delta, delta, delta);
//                    position = Compression.Write7Bit2(buffer, position, delta, delta, delta, delta);
//                    position = Compression.Write7Bit2(buffer, position, delta, delta, delta, delta);
//                    position = Compression.Write7Bit2(buffer, position, delta, delta, delta, delta);
//                    position = Compression.Write7Bit2(buffer, position, delta, delta, delta, delta);
//                    //position = Compression.Compress(buffer, position, next, next+delta);
//                    //next += delta;
//                }
//            }
//            sw.Stop();
//            MessageBox.Show(position.ToString() + "b " + ((sw.Elapsed.TotalMilliseconds * 1000) / loop).ToString());
////            MessageBox.Show(position.ToString() + "b " + ((position / sw.Elapsed.TotalSeconds) * loop / Math.Pow(2, 20)).ToString());
//        }
//        static unsafe void TestRawRead(int delta)
//        {
//            int loop = 10000;
//            byte[] buffer = new byte[65535];
//            int position = 0;

//            for (int repeat = 0; repeat < 1; repeat++)
//            {
//                position = 0;
//                int date = 216532156;
//                int next;
//                for (int x = 0; x < 1000; x++)
//                {
//                    next = date + delta;

//                    fixed (byte* lp = buffer)
//                    {
//                        *(int*)(lp + position) = next;
//                    }
//                    position += 4;
//                    next = date;
//                }
//            }


//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//            for (int repeat = 0; repeat < loop; repeat++)
//            {
//                position = 0;
//                int date = 216532156;
//                int next;
//                for (int x = 0; x < 1000; x++)
//                {
//                    next = date + delta;

//                    fixed (byte* lp = buffer)
//                    {
//                        next = *(int*)(lp + position);
//                    }
//                    position += 4;
//                    next = date;
//                }
//            }
//            sw.Stop();
//            MessageBox.Show(position.ToString() + "b " + ((position / sw.Elapsed.TotalSeconds) * loop / Math.Pow(2, 20)).ToString());
//        }
//        static void TestByteRead(int delta)
//        {
//            int loop = 10000;
//            byte[] buffer = new byte[65535];
//            int position = 0;

//            for (int repeat = 0; repeat < 1; repeat++)
//            {
//                position = 0;
//                int next = 216532156;
//                for (int x = 0; x < 1000; x++)
//                {
//                    position = Compression.Compress(buffer, position, next, next + delta);
//                    next += delta;
//                }
//            }

//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//            for (int repeat = 0; repeat < loop; repeat++)
//            {
//                position = 0;
//                int next = 216532156;
//                for (int x = 0; x < 1000; x++)
//                {
//                    next = Compression.Decompress(buffer, position, next, out position);
//                }
//            }
//            sw.Stop();
//            MessageBox.Show(position.ToString() + "b " + ((position / sw.Elapsed.TotalSeconds) * loop / Math.Pow(2, 20)).ToString());
//        }
//        static void TestCustomClass(uint delta)
//        {
//            CompressBlockUIn32 cmp = new CompressBlockUIn32();
//            int loop = 10000;
//            byte[] buffer = new byte[65535];
//            int position = 0;

//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//            for (int repeat = 0; repeat < loop; repeat++)
//            {
//                cmp.Initialize();
//                position = 0;
//                uint next = 216532156;
//                for (int x = 0; x < 1000; x++)
//                {
//                    position = cmp.AddValue(next,next+delta, buffer, position);
//                    next += delta;
//                }
//                cmp.Flush(buffer);
//            }
//            sw.Stop();
//            MessageBox.Show(position.ToString() + "b " + ((position / sw.Elapsed.TotalSeconds) * loop / Math.Pow(2, 20)).ToString());
//        }

//        static void TestCustomClass2(uint delta)
//        {
//            int loop = 10000;
//            byte[] buffer = new byte[65535];
//            int position = 0;

//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//            for (int repeat = 0; repeat < loop; repeat++)
//            {
//                position = 0;
//                for (int x = 0; x < 50; x++)
//                {
//                    position = CompressBlockUIn32_2.AddValue(delta, delta, delta, delta, buffer, position);
//                    position = CompressBlockUIn32_2.AddValue(delta, delta, delta, delta, buffer, position);
//                    position = CompressBlockUIn32_2.AddValue(delta, delta, delta, delta, buffer, position);
//                    position = CompressBlockUIn32_2.AddValue(delta, delta, delta, delta, buffer, position);
//                    position = CompressBlockUIn32_2.AddValue(delta, delta, delta, delta, buffer, position);
//                }
//            }
//            sw.Stop();
//            MessageBox.Show(position.ToString() + "b " + ((sw.Elapsed.TotalMilliseconds * 1000) / loop).ToString());
//            //MessageBox.Show(position.ToString() + "b " + ((position / sw.Elapsed.TotalSeconds) * loop / Math.Pow(2, 20)).ToString());
//        }

//        static void TestCustomClassStatic(uint delta)
//        {
//            int reservation;
//            int bitPosition;
//            uint oldValue;
//            byte meta=0;

//            CompressBlockUIn32 cmp = new CompressBlockUIn32();
//            int loop = 10000;
//            byte[] buffer = new byte[65535];
//            int position = 0;

//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//            for (int repeat = 0; repeat < loop; repeat++)
//            {
//                CompressBlockUIn32Static.Initialize(out bitPosition, out oldValue, out reservation);
//                position = 0;
//                uint next = 216532156;
//                for (int x = 0; x < 1000; x++)
//                {
//                    next += delta;
//                    position = CompressBlockUIn32Static.AddValue(next, buffer, position,ref meta, ref oldValue, ref bitPosition, ref reservation);
//                }
//                CompressBlockUIn32Static.Flush(buffer, ref meta, ref oldValue, ref bitPosition, ref reservation);
//            }
//            sw.Stop();
//            MessageBox.Show(position.ToString() + "b " + ((position / sw.Elapsed.TotalSeconds) * loop / Math.Pow(2, 20)).ToString());
//        }

//        static unsafe void PinningCost()
//        {
//            int loop = 10000;
//            byte[] buffer = new byte[65535];
//            int position = 0;

//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//            for (int repeat = 0; repeat < loop; repeat++)
//            {
//                position = 0;
//                for (int x = 0; x < 1000; x++)
//                {
//                    int xx = x;
//                    xx = xx << 8 | 1;
//                    xx = xx << 8 | 1;
//                    xx = xx << 8 | 1;
//                    xx = xx << 8 | 1;
//                    //buffer[position++] = 1;
//                    fixed (byte* lp = buffer)
//                    {
//                        *(int*)(lp + position) = xx;
//                    }
//                    position++;
//                }
//            }
//            sw.Stop();
//            MessageBox.Show(position.ToString() + "b " + ((position / sw.Elapsed.TotalSeconds) * loop / Math.Pow(2, 20)).ToString());
//        }
//    }
//}
