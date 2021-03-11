//******************************************************************************************************
//  BinaryStreamWrapperTest.cs - Gbtc
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

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GSF.IO.Unmanaged.Test;
using NUnit.Framework;

namespace GSF.IO.Test
{
    [TestFixture()]
    public class BinaryStreamWrapperTest
    {
        [Test()]
        public void Test()
        {
            MemoryPoolTest.TestMemoryLeak();
            const int count = 1000;
            MemoryStream ms = new MemoryStream();
            ms.Write(new byte[100000], 0, 100000);
            ms.Write(new byte[100000], 0, 100000);
            ms.Position = 0;
            BinaryStreamWrapper bs = new BinaryStreamWrapper(ms, false);
            Stopwatch sw = new Stopwatch();
            //DateTime b = DateTime.UtcNow;
            long b = 10;
            //Guid b = Guid.NewGuid() ;
            for (int x2 = 0; x2 < count; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < count; x++)
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

            sw.Start();
            for (int x2 = 0; x2 < count; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < count; x++)
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
                    //bs.ReadDecimal();
                    //bs.ReadDecimal();
                    //bs.ReadDecimal();
                    //bs.ReadDecimal();
                    //bs.ReadDecimal();
                    //bs.ReadDecimal();
                    //bs.ReadDecimal();
                    //bs.ReadDecimal();
                    //bs.ReadDecimal();
                    //bs.ReadDecimal();
                    //bs.ReadInt64();
                    //bs.ReadInt64();
                    //bs.ReadInt64();
                    //bs.ReadInt64();
                    //bs.ReadInt64();
                    //bs.ReadInt64();
                    //bs.ReadInt64();
                    //bs.ReadInt64();
                    //bs.ReadInt64();
                    //bs.ReadInt64();
                    //bs.ReadInt32();
                    //bs.ReadInt32();
                    //bs.ReadInt32();
                    //bs.ReadInt32();
                    //bs.ReadInt32();
                    //bs.ReadInt32();
                    //bs.ReadInt32();
                    //bs.ReadInt32();
                    //bs.ReadInt32();
                    //bs.ReadInt32();
                    //bs.ReadInt16();
                    //bs.ReadInt16();
                    //bs.ReadInt16();
                    //bs.ReadInt16();
                    //bs.ReadInt16();
                    //bs.ReadInt16();
                    //bs.ReadInt16();
                    //bs.ReadInt16();
                    //bs.ReadInt16();
                    //bs.ReadInt16();
                    //bs.ReadByte();
                    //bs.ReadByte();
                    //bs.ReadByte();
                    //bs.ReadByte();
                    //bs.ReadByte();
                    //bs.ReadByte();
                    //bs.ReadByte();
                    //bs.ReadByte();
                    //bs.ReadByte();
                    //bs.ReadByte();
                    //bs.Write7Bit(b);
                    //bs.Write7Bit(b);
                    //bs.Write7Bit(b);
                    //bs.Write7Bit(b);
                    //bs.Write7Bit(b);
                    //bs.Write7Bit(b);
                    //bs.Write7Bit(b);
                    //bs.Write7Bit(b);
                    //bs.Write7Bit(b);
                    //bs.Write7Bit(b);
                }
            }
            sw.Stop();
            Assert.IsTrue(true);
            ms.Dispose();
            MemoryPoolTest.TestMemoryLeak();

            //MessageBox.Show((count * count * 10 / sw.Elapsed.TotalSeconds / 1000000).ToString());
        }

        [Test()]
        public unsafe void TestMethod()
        {
            MemoryStream stream = new MemoryStream();
            BinaryStreamWrapper bs = new BinaryStreamWrapper(stream, false);
            Random rand = new Random();
            int seed = rand.Next();
            rand = new Random(seed);
            byte[] data = new byte[16];
            byte[] data2 = new byte[16];

            fixed (byte* lp = data)
            {
                for (int x = 0; x < 10000; x++)
                {
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(*lp);
                    int skip = rand.Next(40) + 1;
                    bs.Position += skip;
                    bs.Position -= rand.Next(skip);
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(*(sbyte*)lp);
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(*(short*)lp);
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(*(int*)lp);
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(*(long*)lp);
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(*(ushort*)lp);
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(*(uint*)lp);
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(*(ulong*)lp);
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(NextDecimal(data, rand));
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(*(Guid*)lp);
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(NextDate(data, rand));
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(NextSingle(data, rand));
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(NextDouble(data, rand));
                    rand.NextBytes(data);
                    bool value = *lp != 0;
                    while (rand.Next(4) < 2) bs.Write(value);

                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write7Bit(*(uint*)lp);
                    data[3] = 0;
                    while (rand.Next(4) < 2) bs.Write7Bit(*(uint*)lp);
                    data[2] = 0;
                    while (rand.Next(4) < 2) bs.Write7Bit(*(uint*)lp);
                    data[1] = 0;
                    while (rand.Next(4) < 2) bs.Write7Bit(*(uint*)lp);

                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write7Bit(*(ulong*)lp);
                    data[7] = 0;
                    while (rand.Next(4) < 2) bs.Write7Bit(*(ulong*)lp);
                    data[6] = 0;
                    while (rand.Next(4) < 2) bs.Write7Bit(*(ulong*)lp);
                    data[5] = 0;
                    while (rand.Next(4) < 2) bs.Write7Bit(*(ulong*)lp);
                    data[4] = 0;
                    while (rand.Next(4) < 2) bs.Write7Bit(*(ulong*)lp);
                    data[3] = 0;
                    while (rand.Next(4) < 2) bs.Write7Bit(*(ulong*)lp);
                    data[2] = 0;
                    while (rand.Next(4) < 2) bs.Write7Bit(*(ulong*)lp);
                    data[1] = 0;
                    while (rand.Next(4) < 2) bs.Write7Bit(*(ulong*)lp);

                    rand.NextBytes(data);
                    bs.Write(data, 0, data.Length);

                    while (rand.Next(4) < 2)
                    {
                        if (bs.Position > 100)
                        {
                            bs.Position -= 100;
                            int insertCount = rand.Next(16) + 1;
                            bs.InsertBytes(insertCount, 100);
                            bs.Write(data, 0, insertCount);
                            bs.Position -= insertCount;
                            bs.ReadAll(data2, 0, insertCount);
                            bs.Position -= insertCount;
                            bs.RemoveBytes(insertCount, 100);
                            bs.Position += 100;

                            for (int y = 0; y < insertCount; y++)
                            {
                                if (data[y] != data2[y])
                                    throw new Exception();
                            }
                        }
                    }
                }
                rand = new Random(seed);

                bs.Position = 0;

                for (int x = 0; x < 10000; x++)
                {
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadUInt8() != *lp) throw new Exception();
                    int skip = rand.Next(40) + 1;
                    bs.Position += skip;
                    bs.Position -= rand.Next(skip);
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadInt8() != *(sbyte*)lp) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadInt16() != *(short*)lp) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadInt32() != *(int*)lp) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadInt64() != *(long*)lp) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadUInt16() != *(ushort*)lp) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadUInt32() != *(uint*)lp) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadUInt64() != *(ulong*)lp) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadDecimal() != NextDecimal(data, rand)) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadGuid() != *(Guid*)lp) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadDateTime() != NextDate(data, rand)) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadSingle() != NextSingle(data, rand)) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadDouble() != NextDouble(data, rand)) throw new Exception();
                    rand.NextBytes(data);
                    bool b2 = *lp != 0;
                    while (rand.Next(4) < 2) if (bs.ReadBoolean() != b2) throw new Exception();

                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt32() != *(uint*)lp) throw new Exception();
                    data[3] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt32() != *(uint*)lp) throw new Exception();
                    data[2] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt32() != *(uint*)lp) throw new Exception();
                    data[1] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt32() != *(uint*)lp) throw new Exception();

                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != *(ulong*)lp) throw new Exception();
                    data[7] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != *(ulong*)lp) throw new Exception();
                    data[6] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != *(ulong*)lp) throw new Exception();
                    data[5] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != *(ulong*)lp) throw new Exception();
                    data[4] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != *(ulong*)lp) throw new Exception();
                    data[3] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != *(ulong*)lp) throw new Exception();
                    data[2] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != *(ulong*)lp) throw new Exception();
                    data[1] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != *(ulong*)lp) throw new Exception();

                    rand.NextBytes(data);
                    bs.ReadAll(data2, 0, 16);
                    if (!data2.SequenceEqual(data)) throw new Exception();

                    while (rand.Next(4) < 2)
                    {
                        if (bs.Position > 100)
                        {
                            int insertCount = rand.Next(16);
                        }
                    }
                }
            }
        }

        private static unsafe float NextSingle(byte[] data, Random rand)
        {
            fixed (byte* lp = data)
            {
                float value = *(float*)lp;
                while (float.IsInfinity(value) || float.IsNaN(value))
                {
                    rand.NextBytes(data);
                    value = *(float*)lp;
                }
                return value;
            }
        }

        private static unsafe double NextDouble(byte[] data, Random rand)
        {
            fixed (byte* lp = data)
            {
                double value = *(double*)lp;
                while (double.IsInfinity(value) || double.IsNaN(value))
                {
                    rand.NextBytes(data);
                    value = *(double*)lp;
                }
                return value;
            }
        }

        private static unsafe decimal NextDecimal(byte[] data, Random rand)
        {
            return (decimal)(rand.Next() * 0.02);
        }

        private static unsafe DateTime NextDate(byte[] data, Random rand)
        {
            return new DateTime(rand.Next() * 1000L + DateTime.MinValue.Ticks);
        }
    }
}