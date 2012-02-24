using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace Historian.StorageSystem
{
    class BinaryStreamTest
    {
        public static void Test()
        {
            const int count = 1000;
            PooledMemoryStream ms = new PooledMemoryStream();
            ms.Write(new byte[100000], 0, 100000);
            ms.Write(new byte[100000], 0, 100000);
            ms.Position = 0;
            BinaryStream bs = new BinaryStream(ms);
            Stopwatch sw = new Stopwatch();
            //DateTime b = DateTime.UtcNow;
            uint b = 10;
            //Guid b = Guid.NewGuid() ;
            sw.Start();
            for (int x2 = 0; x2 < count; x2++)
            {
                bs.Position = 0;
                for (int x = 0; x < count; x++)
                {
                    //bs.Write(b, b, b, b);
                    //bs.Write(b, b, b, b);
                    //bs.Write(b, b, b, b);
                    //bs.Write(b, b, b, b);
                    //bs.Write(b, b, b, b);
                    //bs.Write(b, b, b, b);
                    //bs.Write(b, b, b, b);
                    //bs.Write(b, b, b, b);
                    //bs.Write(b, b, b, b);
                    //bs.Write(b, b, b, b);
                    //bs.Write(b, b, b);
                    //bs.Write(b, b, b);
                    //bs.Write(b, b, b);
                    //bs.Write(b, b, b);
                    //bs.Write(b, b, b);
                    //bs.Write(b, b, b);
                    //bs.Write(b, b, b);
                    //bs.Write(b, b, b);
                    //bs.Write(b, b, b);
                    //bs.Write(b, b, b);
                    //bs.Write(b, b);
                    //bs.Write(b, b);
                    //bs.Write(b, b);
                    //bs.Write(b, b);
                    //bs.Write(b, b);
                    //bs.Write(b, b);
                    //bs.Write(b, b);
                    //bs.Write(b, b);
                    //bs.Write(b, b);
                    //bs.Write(b, b);
                    //bs.Write(b);
                    //bs.Write(b);
                    //bs.Write(b);
                    //bs.Write(b);
                    //bs.Write(b);
                    //bs.Write(b);
                    //bs.Write(b);
                    //bs.Write(b);
                    //bs.Write(b);
                    //bs.Write(b);
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
            MessageBox.Show((count * count * 10 / sw.Elapsed.TotalSeconds / 1000000).ToString());

        }

        public unsafe static void Test(ISupportsBinaryStream stream)
        {
            BinaryStream bs = new BinaryStream(stream);
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
                    while (rand.Next(4) < 2) bs.Write(*(byte*)lp);
                    bs.Position += rand.Next(40);
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
                    while (rand.Next(4) < 2) bs.Write(*(decimal*)lp);
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(*(Guid*)lp);
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(*(DateTime*)lp);
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(NextSingle(data, rand));
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) bs.Write(NextDouble(data, rand));
                    rand.NextBytes(data);
                    bool value = (*lp != 0);
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
                    bs.Write(data,0,data.Length);

                    while (rand.Next(4) < 2)
                    {
                        if (bs.Position>100)
                        {
                            bs.Position-=100;
                            int insertCount=rand.Next(16)+1;
                            bs.InsertBytes(insertCount, 100);
                            bs.Write(data, 0, insertCount);
                            bs.Position -= insertCount;
                            bs.Read(data2, 0, insertCount);
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

                bs.FlushToUnderlyingStream();
                stream.Position = 0;

                for (int x = 0; x < 10000; x++)
                {
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadByte() != (*(byte*)lp)) throw new Exception();
                    bs.Position += rand.Next(40);
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadSByte() != (*(sbyte*)lp)) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadInt16() != (*(short*)lp)) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadInt32() != (*(int*)lp)) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadInt64() != (*(long*)lp)) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadUInt16() != (*(ushort*)lp)) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadUInt32() != (*(uint*)lp)) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadUInt64() != (*(ulong*)lp)) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadDecimal() != (*(decimal*)lp)) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadGuid() != (*(Guid*)lp)) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadDateTime() != (*(DateTime*)lp)) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadSingle() != NextSingle(data, rand)) throw new Exception();
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.ReadDouble() != NextDouble(data, rand)) throw new Exception();
                    rand.NextBytes(data);
                    bool b2 = (*lp != 0);
                    while (rand.Next(4) < 2) if (bs.ReadBoolean() != b2) throw new Exception();
                    
                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt32() != (*(uint*)lp)) throw new Exception();
                    data[3] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt32() != (*(uint*)lp)) throw new Exception();
                    data[2] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt32() != (*(uint*)lp)) throw new Exception();
                    data[1] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt32() != (*(uint*)lp)) throw new Exception();

                    rand.NextBytes(data);
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != (*(ulong*)lp)) throw new Exception();
                    data[7] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != (*(ulong*)lp)) throw new Exception();
                    data[6] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != (*(ulong*)lp)) throw new Exception();
                    data[5] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != (*(ulong*)lp)) throw new Exception();
                    data[4] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != (*(ulong*)lp)) throw new Exception();
                    data[3] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != (*(ulong*)lp)) throw new Exception();
                    data[2] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != (*(ulong*)lp)) throw new Exception();
                    data[1] = 0;
                    while (rand.Next(4) < 2) if (bs.Read7BitUInt64() != (*(ulong*)lp)) throw new Exception();
                    
                    rand.NextBytes(data);
                    bs.Read(data2, 0, 16);
                    if (!data2.SequenceEqual<byte>(data)) throw new Exception();

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
        unsafe static float NextSingle(byte[] data, Random rand)
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
        unsafe static double NextDouble(byte[] data, Random rand)
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
    }
}
