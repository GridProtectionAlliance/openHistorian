//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Diagnostics;
//using System.Windows.Forms;

//namespace openHistorian.Core.Unmanaged
//{
//    class BinaryStream2Benchmark
//    {
//        public static void Run()
//        {
//            const int count = 10000;
//            MemoryStream ms = new MemoryStream();
//            BinaryStreamOld bs = new BinaryStreamOld(ms);
            
//            StringBuilder sb = new StringBuilder();
//            sb.AppendLine(RunSeeks(count, bs));
//            sb.AppendLine("Type\tRead\tWrite");
//            sb.AppendLine(RunByte(count, bs));
//            sb.AppendLine(RunSByte(count, bs));
//            sb.AppendLine(RunUShort(count, bs));
//            sb.AppendLine(RunShort(count, bs));
//            sb.AppendLine(RunUInt(count, bs));
//            sb.AppendLine(RunInt(count, bs));
//            sb.AppendLine(RunSingle(count, bs));
//            sb.AppendLine(RunULong(count, bs));
//            sb.AppendLine(RunLong(count, bs));
//            sb.AppendLine(RunDouble(count, bs));

//            Clipboard.SetText(sb.ToString());
//            MessageBox.Show(sb.ToString());

//        }

//        public static string RunSeeks(int thousands, BinaryStreamOld bs)
//        {
//            Stopwatch sw1 = new Stopwatch();
//            byte b = 10;

//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 100;
//                bs.Write(b);

//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Position = 0;
//                    bs.Position = 1;
//                    bs.Position = 2;
//                    bs.Position = 3;
//                    bs.Position = 4;
//                    bs.Position = 5;
//                    bs.Position = 6;
//                    bs.Position = 7;
//                    bs.Position = 8;
//                    bs.Position = 9;
//                }
//            }

//            sw1.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 100;
//                bs.Write(b);

//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Position = 0;
//                    bs.Position = 1;
//                    bs.Position = 2;
//                    bs.Position = 3;
//                    bs.Position = 4;
//                    bs.Position = 5;
//                    bs.Position = 6;
//                    bs.Position = 7;
//                    bs.Position = 8;
//                    bs.Position = 9;
//                }
//            }
//            sw1.Stop();

//            return "Seeks\t" + (thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000);
//        }

//        public static string RunULong(int thousands, BinaryStreamOld bs)
//        {
//            Stopwatch sw1 = new Stopwatch();
//            Stopwatch sw2 = new Stopwatch();
//            ulong b = 10;

//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }

//            sw1.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }
//            sw1.Stop();

//            sw2.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.ReadUInt64();
//                    bs.ReadUInt64();
//                    bs.ReadUInt64();
//                    bs.ReadUInt64();
//                    bs.ReadUInt64();
//                    bs.ReadUInt64();
//                    bs.ReadUInt64();
//                    bs.ReadUInt64();
//                    bs.ReadUInt64();
//                    bs.ReadUInt64();
//                }
//            }
//            sw2.Stop();
//            return "ULong\t" + (thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000) + "\t" +
//                (thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000);
//        }

//        public static string RunLong(int thousands, BinaryStreamOld bs)
//        {
//            Stopwatch sw1 = new Stopwatch();
//            Stopwatch sw2 = new Stopwatch();
//            long b = 10;

//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }

//            sw1.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }
//            sw1.Stop();

//            sw2.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.ReadInt64();
//                    bs.ReadInt64();
//                    bs.ReadInt64();
//                    bs.ReadInt64();
//                    bs.ReadInt64();
//                    bs.ReadInt64();
//                    bs.ReadInt64();
//                    bs.ReadInt64();
//                    bs.ReadInt64();
//                    bs.ReadInt64();
//                }
//            }
//            sw2.Stop();
//            return "Long\t" + (thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000) + "\t" +
//                (thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000);
//        }

//        public static string RunDouble(int thousands, BinaryStreamOld bs)
//        {
//            Stopwatch sw1 = new Stopwatch();
//            Stopwatch sw2 = new Stopwatch();
//            double b = 10;

//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }

//            sw1.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }
//            sw1.Stop();

//            sw2.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.ReadDouble();
//                    bs.ReadDouble();
//                    bs.ReadDouble();
//                    bs.ReadDouble();
//                    bs.ReadDouble();
//                    bs.ReadDouble();
//                    bs.ReadDouble();
//                    bs.ReadDouble();
//                    bs.ReadDouble();
//                    bs.ReadDouble();
//                }
//            }
//            sw2.Stop();
//            return "Double\t" + (thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000) + "\t" +
//                (thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000);
//        }

//        public static string RunUInt(int thousands, BinaryStreamOld bs)
//        {
//            Stopwatch sw1 = new Stopwatch();
//            Stopwatch sw2 = new Stopwatch();
//            uint b = 10;

//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }

//            sw1.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }
//            sw1.Stop();

//            sw2.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.ReadUInt32();
//                    bs.ReadUInt32();
//                    bs.ReadUInt32();
//                    bs.ReadUInt32();
//                    bs.ReadUInt32();
//                    bs.ReadUInt32();
//                    bs.ReadUInt32();
//                    bs.ReadUInt32();
//                    bs.ReadUInt32();
//                    bs.ReadUInt32();
//                }
//            }
//            sw2.Stop();
//            return "UInt\t" + (thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000) + "\t" +
//                (thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000);
//        }

//        public static string RunInt(int thousands, BinaryStreamOld bs)
//        {
//            Stopwatch sw1 = new Stopwatch();
//            Stopwatch sw2 = new Stopwatch();
//            int b = 10;

//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }

//            sw1.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }
//            sw1.Stop();

//            sw2.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.ReadInt32();
//                    bs.ReadInt32();
//                    bs.ReadInt32();
//                    bs.ReadInt32();
//                    bs.ReadInt32();
//                    bs.ReadInt32();
//                    bs.ReadInt32();
//                    bs.ReadInt32();
//                    bs.ReadInt32();
//                    bs.ReadInt32();
//                }
//            }
//            sw2.Stop();
//            return "Int\t" + (thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000) + "\t" +
//                (thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000);
//        }

//        public static string RunSingle(int thousands, BinaryStreamOld bs)
//        {
//            Stopwatch sw1 = new Stopwatch();
//            Stopwatch sw2 = new Stopwatch();
//            float b = 10;

//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }

//            sw1.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }
//            sw1.Stop();

//            sw2.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.ReadSingle();
//                    bs.ReadSingle();
//                    bs.ReadSingle();
//                    bs.ReadSingle();
//                    bs.ReadSingle();
//                    bs.ReadSingle();
//                    bs.ReadSingle();
//                    bs.ReadSingle();
//                    bs.ReadSingle();
//                    bs.ReadSingle();
//                }
//            }
//            sw2.Stop();
//            return "Single\t" + (thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000) + "\t" +
//                (thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000);
//        }

//        public static string RunUShort(int thousands, BinaryStreamOld bs)
//        {
//            Stopwatch sw1 = new Stopwatch();
//            Stopwatch sw2 = new Stopwatch();
//            ushort b = 10;

//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }

//            sw1.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }
//            sw1.Stop();

//            sw2.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.ReadUInt16();
//                    bs.ReadUInt16();
//                    bs.ReadUInt16();
//                    bs.ReadUInt16();
//                    bs.ReadUInt16();
//                    bs.ReadUInt16();
//                    bs.ReadUInt16();
//                    bs.ReadUInt16();
//                    bs.ReadUInt16();
//                    bs.ReadUInt16();
//                }
//            }
//            sw2.Stop();
//            return "UShort\t" + (thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000) + "\t" +
//                (thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000);
//        }

//        public static string RunShort(int thousands, BinaryStreamOld bs)
//        {
//            Stopwatch sw1 = new Stopwatch();
//            Stopwatch sw2 = new Stopwatch();
//            short b = 10;

//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }

//            sw1.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }
//            sw1.Stop();

//            sw2.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.ReadInt16();
//                    bs.ReadInt16();
//                    bs.ReadInt16();
//                    bs.ReadInt16();
//                    bs.ReadInt16();
//                    bs.ReadInt16();
//                    bs.ReadInt16();
//                    bs.ReadInt16();
//                    bs.ReadInt16();
//                    bs.ReadInt16();
//                }
//            }
//            sw2.Stop();
//            return "Short\t" + (thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000) + "\t" +
//                (thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000);
//        }

//        public static string RunSByte(int thousands, BinaryStreamOld bs)
//        {
//            Stopwatch sw1 = new Stopwatch();
//            Stopwatch sw2 = new Stopwatch();
//            sbyte b = 10;

//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }

//            sw1.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }
//            sw1.Stop();

//            sw2.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.ReadSByte();
//                    bs.ReadSByte();
//                    bs.ReadSByte();
//                    bs.ReadSByte();
//                    bs.ReadSByte();
//                    bs.ReadSByte();
//                    bs.ReadSByte();
//                    bs.ReadSByte();
//                    bs.ReadSByte();
//                    bs.ReadSByte();
//                }
//            }
//            sw2.Stop();
//            return "SByte\t" + (thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000) + "\t" +
//                (thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000);
//        }

//        public static string RunByte(int thousands, BinaryStreamOld bs)
//        {
//            Stopwatch sw1 = new Stopwatch();
//            Stopwatch sw2 = new Stopwatch();
//            byte b = 10;

//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }

//            sw1.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                    bs.Write(b);
//                }
//            }
//            sw1.Stop();

//            sw2.Start();
//            for (int x2 = 0; x2 < thousands; x2++)
//            {
//                bs.Position = 0;
//                for (int x = 0; x < 100; x++)
//                {
//                    bs.ReadByte();
//                    bs.ReadByte();
//                    bs.ReadByte();
//                    bs.ReadByte();
//                    bs.ReadByte();
//                    bs.ReadByte();
//                    bs.ReadByte();
//                    bs.ReadByte();
//                    bs.ReadByte();
//                    bs.ReadByte();
//                }
//            }
//            sw2.Stop();
//            return "Byte\t" + (thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000) + "\t" +
//                (thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000);
//        }

//    }
//}
