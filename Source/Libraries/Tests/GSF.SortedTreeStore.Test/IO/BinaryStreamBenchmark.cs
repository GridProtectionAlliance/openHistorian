//******************************************************************************************************
//  BinaryStreamBenchmark.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using GSF.IO.Unmanaged;
using GSF.IO.Unmanaged.Test;
using NUnit.Framework;

namespace GSF.IO.Test
{
    [TestFixture]
    public class BinaryStreamBenchmark
    {
        [Test]
        public void RunMemoryStreamTest()
        {
            MemoryPoolTest.TestMemoryLeak();
            MemoryPoolStream ms = new MemoryPoolStream();
            BinaryStream bs = new BinaryStream(ms);
            Run(bs, false);
            ms.Dispose();
            MemoryPoolTest.TestMemoryLeak();
        }

        public static void Run(BinaryStream bs, bool clipboard = true)
        {
            const int count = 100;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(RunInserts(count, bs));
            sb.AppendLine(RunSeeks(count, bs));
            sb.AppendLine(GetWritePointer(bs));
            sb.AppendLine(GetReadPointer(count, bs));
            sb.AppendLine("Type\tRead\tWrite");
            sb.AppendLine(RunByte(count, bs));
            sb.AppendLine(RunSByte(count, bs));
            sb.AppendLine(RunUShort(count, bs));
            sb.AppendLine(RunShort(count, bs));
            sb.AppendLine(RunUInt(count, bs));
            sb.AppendLine(RunInt(count, bs));
            sb.AppendLine(RunSingle(count, bs));
            sb.AppendLine(RunULong(count, bs));
            sb.AppendLine(RunLong(count, bs));
            sb.AppendLine(RunDouble(count, bs));
            sb.AppendLine(Run7Bit32(count, bs, 10));
            sb.AppendLine(Run7Bit32(count, bs, 128 + 10));
            sb.AppendLine(Run7Bit32(count, bs, 128 * 128 + 10));
            sb.AppendLine(Run7Bit32(count, bs, 128 * 128 * 128 + 10));
            sb.AppendLine(Run7Bit32(count, bs, 128 * 128 * 128 * 128 + 10));
            sb.AppendLine(Run7Bit64(count, bs, 10));
            sb.AppendLine(Run7Bit64(count, bs, 128 + 10));
            sb.AppendLine(Run7Bit64(count, bs, 128 * 128 + 10));
            sb.AppendLine(Run7Bit64(count, bs, 128 * 128 * 128 + 10));
            sb.AppendLine(Run7Bit64(count, bs, 128L * 128L * 128L * 128L + 10));
            sb.AppendLine(Run7Bit64(count, bs, 128L * 128L * 128L * 128L * 128L + 10));
            sb.AppendLine(Run7Bit64(count, bs, 128L * 128L * 128L * 128L * 128L * 128L + 10));
            sb.AppendLine(Run7Bit64(count, bs, 128L * 128L * 128L * 128L * 128L * 128L * 128L + 10));
            sb.AppendLine(Run7Bit64(count, bs, 128L * 128L * 128L * 128L * 128L * 128L * 128L * 128L + 10));
            sb.AppendLine(Run7Bit64(count, bs, ulong.MaxValue));

            if (clipboard)
            {
                Clipboard.SetText(sb.ToString());
                MessageBox.Show(sb.ToString());
            }
            else
            {
                System.Console.WriteLine(sb.ToString());
            }
        }

        public static string RunInserts(int thousands, BinaryStream bs)
        {
            const int insertBytes = 1;
            const int moveSize = 512;
            Stopwatch sw1 = new Stopwatch();
            byte b = 10;

            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                bs.Write(b);

                for (int x = 0; x < 100; x++)
                {
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                }
            }

            sw1.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                bs.Write(b);

                for (int x = 0; x < 100; x++)
                {
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                    bs.InsertBytes(insertBytes, moveSize);
                }
            }
            sw1.Stop();

            return "Inserts\t" + thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000;
        }

        public static unsafe string GetReadPointer(int thousands, BinaryStream bs)
        {
            return "GetReadPointer\t" +
                   StepTimer.Time(10, () =>
                   {
                       bs.GetReadPointer(40 * 1000000, 1);
                       bs.GetReadPointer(80 * 1000000, 1);
                       bs.GetReadPointer(120 * 1000000, 1);
                       bs.GetReadPointer(50 * 1000000, 1);
                       bs.GetReadPointer(90 * 1000000, 1);
                       bs.GetReadPointer(130 * 1000000, 1);
                       bs.GetReadPointer(60 * 1000000, 1);
                       bs.GetReadPointer(100 * 1000000, 1);
                       bs.GetReadPointer(140 * 1000000, 1);
                       bs.GetReadPointer(110 * 1000000, 1);
                   });
        }

        public static unsafe string GetWritePointer(BinaryStream bs)
        {
            return "GetWritePointer\t" +
                   StepTimer.Time(10, () =>
                   {
                       bs.GetWritePointer(40 * 1000000, 1);
                       bs.GetWritePointer(80 * 1000000, 1);
                       bs.GetWritePointer(120 * 1000000, 1);
                       bs.GetWritePointer(50 * 1000000, 1);
                       bs.GetWritePointer(90 * 1000000, 1);
                       bs.GetWritePointer(130 * 1000000, 1);
                       bs.GetWritePointer(60 * 1000000, 1);
                       bs.GetWritePointer(100 * 1000000, 1);
                       bs.GetWritePointer(140 * 1000000, 1);
                       bs.GetWritePointer(110 * 1000000, 1);
                   });
        }

        public static string RunSeeks(int thousands, BinaryStream bs)
        {
            Stopwatch sw1 = new Stopwatch();
            byte b = 10;

            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 100;
                bs.Write(b);

                for (int x = 0; x < 100; x++)
                {
                    bs.Position = 0;
                    bs.Position = 1;
                    bs.Position = 2;
                    bs.Position = 3;
                    bs.Position = 4;
                    bs.Position = 5;
                    bs.Position = 6;
                    bs.Position = 7;
                    bs.Position = 8;
                    bs.Position = 9;
                }
            }

            sw1.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 100;
                bs.Write(b);

                for (int x = 0; x < 100; x++)
                {
                    bs.Position = 0;
                    bs.Position = 1;
                    bs.Position = 2;
                    bs.Position = 3;
                    bs.Position = 4;
                    bs.Position = 5;
                    bs.Position = 6;
                    bs.Position = 7;
                    bs.Position = 8;
                    bs.Position = 9;
                }
            }
            sw1.Stop();

            return "Seeks\t" + thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000;
        }

        public static string RunULong(int thousands, BinaryStream bs)
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            ulong b = 10;

            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }

            sw1.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }
            sw1.Stop();

            sw2.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.ReadUInt64();
                    bs.ReadUInt64();
                    bs.ReadUInt64();
                    bs.ReadUInt64();
                    bs.ReadUInt64();
                    bs.ReadUInt64();
                    bs.ReadUInt64();
                    bs.ReadUInt64();
                    bs.ReadUInt64();
                    bs.ReadUInt64();
                }
            }
            sw2.Stop();
            return "ULong\t" + thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000 + "\t" +
                   thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000;
        }

        public static string RunLong(int thousands, BinaryStream bs)
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            long b = 10;

            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }

            sw1.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }
            sw1.Stop();

            sw2.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.ReadInt64();
                    bs.ReadInt64();
                    bs.ReadInt64();
                    bs.ReadInt64();
                    bs.ReadInt64();
                    bs.ReadInt64();
                    bs.ReadInt64();
                    bs.ReadInt64();
                    bs.ReadInt64();
                    bs.ReadInt64();
                }
            }
            sw2.Stop();
            return "Long\t" + thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000 + "\t" +
                   thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000;
        }

        public static string RunDouble(int thousands, BinaryStream bs)
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            double b = 10;

            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }

            sw1.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }
            sw1.Stop();

            sw2.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.ReadDouble();
                    bs.ReadDouble();
                    bs.ReadDouble();
                    bs.ReadDouble();
                    bs.ReadDouble();
                    bs.ReadDouble();
                    bs.ReadDouble();
                    bs.ReadDouble();
                    bs.ReadDouble();
                    bs.ReadDouble();
                }
            }
            sw2.Stop();
            return "Double\t" + thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000 + "\t" +
                   thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000;
        }

        public static string Run7Bit64(int thousands, BinaryStream bs, ulong b)
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();

            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                }
            }

            sw1.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                }
            }
            sw1.Stop();

            sw2.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Read7BitUInt64();
                    bs.Read7BitUInt64();
                    bs.Read7BitUInt64();
                    bs.Read7BitUInt64();
                    bs.Read7BitUInt64();
                    bs.Read7BitUInt64();
                    bs.Read7BitUInt64();
                    bs.Read7BitUInt64();
                    bs.Read7BitUInt64();
                    bs.Read7BitUInt64();
                }
            }
            sw2.Stop();
            return "7Bit64 " + Encoding7Bit.GetSize(b) + "\t" + thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000 + "\t" +
                   thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000;
        }

        public static string Run7Bit32(int thousands, BinaryStream bs, uint b)
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();

            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                }
            }

            sw1.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                    bs.Write7Bit(b);
                }
            }
            sw1.Stop();

            sw2.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Read7BitUInt32();
                    bs.Read7BitUInt32();
                    bs.Read7BitUInt32();
                    bs.Read7BitUInt32();
                    bs.Read7BitUInt32();
                    bs.Read7BitUInt32();
                    bs.Read7BitUInt32();
                    bs.Read7BitUInt32();
                    bs.Read7BitUInt32();
                    bs.Read7BitUInt32();
                }
            }
            sw2.Stop();
            return "7Bit32 " + Encoding7Bit.GetSize(b) + "\t" + thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000 + "\t" +
                   thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000;
        }

        public static string RunUInt(int thousands, BinaryStream bs)
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            uint b = 10;

            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }

            sw1.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }
            sw1.Stop();

            sw2.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.ReadUInt32();
                    bs.ReadUInt32();
                    bs.ReadUInt32();
                    bs.ReadUInt32();
                    bs.ReadUInt32();
                    bs.ReadUInt32();
                    bs.ReadUInt32();
                    bs.ReadUInt32();
                    bs.ReadUInt32();
                    bs.ReadUInt32();
                }
            }
            sw2.Stop();
            return "UInt\t" + thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000 + "\t" +
                   thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000;
        }

        public static string RunInt(int thousands, BinaryStream bs)
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            int b = 10;

            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }

            sw1.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }
            sw1.Stop();

            sw2.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.ReadInt32();
                    bs.ReadInt32();
                    bs.ReadInt32();
                    bs.ReadInt32();
                    bs.ReadInt32();
                    bs.ReadInt32();
                    bs.ReadInt32();
                    bs.ReadInt32();
                    bs.ReadInt32();
                    bs.ReadInt32();
                }
            }
            sw2.Stop();
            return "Int\t" + thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000 + "\t" +
                   thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000;
        }

        public static string RunSingle(int thousands, BinaryStream bs)
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            float b = 10;

            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }

            sw1.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }
            sw1.Stop();

            sw2.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.ReadSingle();
                    bs.ReadSingle();
                    bs.ReadSingle();
                    bs.ReadSingle();
                    bs.ReadSingle();
                    bs.ReadSingle();
                    bs.ReadSingle();
                    bs.ReadSingle();
                    bs.ReadSingle();
                    bs.ReadSingle();
                }
            }
            sw2.Stop();
            return "Single\t" + thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000 + "\t" +
                   thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000;
        }

        public static string RunUShort(int thousands, BinaryStream bs)
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            ushort b = 10;

            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }

            sw1.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }
            sw1.Stop();

            sw2.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.ReadUInt16();
                    bs.ReadUInt16();
                    bs.ReadUInt16();
                    bs.ReadUInt16();
                    bs.ReadUInt16();
                    bs.ReadUInt16();
                    bs.ReadUInt16();
                    bs.ReadUInt16();
                    bs.ReadUInt16();
                    bs.ReadUInt16();
                }
            }
            sw2.Stop();
            return "UShort\t" + thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000 + "\t" +
                   thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000;
        }

        public static string RunShort(int thousands, BinaryStream bs)
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            short b = 10;

            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }

            sw1.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }
            sw1.Stop();

            sw2.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.ReadInt16();
                    bs.ReadInt16();
                    bs.ReadInt16();
                    bs.ReadInt16();
                    bs.ReadInt16();
                    bs.ReadInt16();
                    bs.ReadInt16();
                    bs.ReadInt16();
                    bs.ReadInt16();
                    bs.ReadInt16();
                }
            }
            sw2.Stop();
            return "Short\t" + thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000 + "\t" +
                   thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000;
        }

        public static string RunSByte(int thousands, BinaryStream bs)
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            sbyte b = 10;

            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }

            sw1.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }
            sw1.Stop();

            sw2.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.ReadInt8();
                    bs.ReadInt8();
                    bs.ReadInt8();
                    bs.ReadInt8();
                    bs.ReadInt8();
                    bs.ReadInt8();
                    bs.ReadInt8();
                    bs.ReadInt8();
                    bs.ReadInt8();
                    bs.ReadInt8();
                }
            }
            sw2.Stop();
            return "SByte\t" + thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000 + "\t" +
                   thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000;
        }

        public static string RunByte(int thousands, BinaryStream bs)
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            byte b = 10;

            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }

            sw1.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                    bs.Write(b);
                }
            }
            sw1.Stop();

            sw2.Start();
            for (int x2 = 0; x2 < thousands; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < 100; x++)
                {
                    bs.ReadUInt8();
                    bs.ReadUInt8();
                    bs.ReadUInt8();
                    bs.ReadUInt8();
                    bs.ReadUInt8();
                    bs.ReadUInt8();
                    bs.ReadUInt8();
                    bs.ReadUInt8();
                    bs.ReadUInt8();
                    bs.ReadUInt8();
                }
            }
            sw2.Stop();
            return "Byte\t" + thousands * 1000 / sw2.Elapsed.TotalSeconds / 1000000 + "\t" +
                   thousands * 1000 / sw1.Elapsed.TotalSeconds / 1000000;
        }
    }
}