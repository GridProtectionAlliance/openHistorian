//using System;
//using System.IO;
//using System.Runtime.CompilerServices;

//namespace openHistorian.Core
//{
//    public class Compression
//    {
//        public static void Compress(Stream stream, long previousValue, long currentValue)
//        {
//            long diff = previousValue ^ currentValue;
//            Write7Bit((ulong)diff, stream);
//        }
//        public static int Compress(byte[] stream, int position, int previousValue, int currentValue)
//        {
//            int diff = previousValue ^ currentValue;
//            return Write7Bit((uint)diff, stream, position);
//        }

//        public static int Decompress(byte[] stream, int position, int previousValue, out int newPosition)
//        {
//            int diff = (int)Read7BitUInt32(stream,position,out newPosition);
//            return previousValue ^ diff;
//        }

//        public static void Compress(Stream stream, int previousValue, int currentValue)
//        {
//            int diff = previousValue ^ currentValue;
//            Write7Bit((uint)diff, stream);
//        }
//        public static int Decompress(Stream stream, int previousValue)
//        {
//            int diff = (int)Read7BitUInt32(stream);
//            return previousValue ^ diff;
//        }
//        public static long Decompress(Stream stream, long previousValue)
//        {
//            long diff = (long)Read7BitUInt64(stream);
//            return previousValue ^ diff;
//        }

//        //public unsafe static int Write7Bit(uint value, byte[] stream, int position)
//        //{
//        //    int newPosition;
//        //    uint b0 = (byte)value;
//        //    uint b1 = (byte)(value >> 7);
//        //    uint b2 = (byte)(value >> 14);
//        //    uint b3 = (byte)(value >> 21);
//        //    ulong b4 = (byte)(value >> 28);

//        //    ulong data;
//        //    if (value < 128)
//        //    {
//        //        newPosition = position + 1;
//        //        data = b0;
//        //    }
//        //    else if (value < 128*128)
//        //    {
//        //        newPosition = position + 2;
//        //        data = (b1 << 8) | b0 | 0x80;
//        //    }
//        //    else if (value < 128 * 128 * 128)
//        //    {
//        //        newPosition = position + 3;
//        //        data = (b2 << 16) | (b1 << 8) | b0 | 0x8080;
//        //    }
//        //    else if (value < 128 * 128 * 128 * 128)
//        //    {
//        //        newPosition = position + 4;
//        //        data = (b3 << 24) | (b2 << 16) | (b1 << 8) | b0 | 0x808080;
//        //    }
//        //    else
//        //    {
//        //        newPosition = position + 5;
//        //        data = (b4 << 32) | (b3 << 24) | (b2 << 16) | (b1 << 8) | b0 | 0x80808080;
//        //    }

//        //    fixed (byte* lp = stream)
//        //    {
//        //        *(ulong*)(lp+position)=data;
//        //    }
//        //    return newPosition;
//        //}

//        //public unsafe static int Write7Bit(uint value, byte[] stream, int position)
//        //{
//        //    fixed (byte* lp = stream)
//        //    {
//        //        byte* lp2 = lp;
//        //        lp2 += position;
//        //        while (value > 127)
//        //        {
//        //            //Set bit 7 to true indicating more
//        //            *lp2 = (byte)(value | 128);
//        //            lp2++;
//        //            value = value >> 7;
//        //        }
//        //        *lp2 = (byte)(value);
//        //        return (int)(lp2 - lp) + 1;
//        //    }
//        //}

//        public unsafe static int Write7Bit(uint value, byte[] stream, int position)
//        {
//            fixed (byte* lp = stream)
//            {
//                byte* lp2 = lp;
//                lp2 += position;
//                while (value > 127)
//                {
//                    //Set bit 7 to true indicating more
//                    *lp2 = (byte)(value | 128);
//                    lp2++;
//                    value = value >> 7;
//                }
//                *lp2 = (byte)(value);
//                return (int)(lp2 - lp) + 1;
//            }
//        }

//        public unsafe static int Write7Bit(byte[] stream, int position, uint value1, uint value2, uint value3, uint value4)
//        {
//            fixed (byte* lp = stream)
//            {
//                byte* lp2 = lp;
//                lp2 += position;

//                while (value1 > 127)
//                {
//                    *lp2 = (byte)(value1 | 128);
//                    lp2++;
//                    value1 = value1 >> 7;
//                }
//                *lp2 = (byte)(value1);
//                lp2++;

//                return (int)(lp2 - lp);
//            }
//        }
//        //public unsafe static int Write7Bit2(byte[] stream, int position, uint value1, uint value2, uint value3, uint value4)
//        //{
//        //    fixed (byte* lp = stream)
//        //    {
//        //        byte* lp2 = lp;
//        //        lp2 += position;

//        //        while (value1 > 127)
//        //        {
//        //            //Set bit 7 to true indicating more
//        //            *lp2 = (byte)(value1 | 128);
//        //            lp2++;
//        //            value1 = value1 >> 7;
//        //        }
//        //        *lp2 = (byte)(value1);
//        //        lp2++;

//        //        while (value2 > 127)
//        //        {
//        //            //Set bit 7 to true indicating more
//        //            *lp2 = (byte)(value2 | 128);
//        //            lp2++;
//        //            value2 = value2 >> 7;
//        //        }
//        //        *lp2 = (byte)(value3);
//        //        lp2++;

//        //        while (value3 > 127)
//        //        {
//        //            //Set bit 7 to true indicating more
//        //            *lp2 = (byte)(value3 | 128);
//        //            lp2++;
//        //            value3 = value3 >> 7;
//        //        }
//        //        *lp2 = (byte)(value3);
//        //        lp2++;

//        //        while (value4 > 127)
//        //        {
//        //            //Set bit 7 to true indicating more
//        //            *lp2 = (byte)(value4 | 128);
//        //            lp2++;
//        //            value4 = value4 >> 7;
//        //        }
//        //        *lp2 = (byte)(value4);
//        //        lp2++;

//        //        return (int)(lp2 - lp);
//        //    }
//        //}

//        public static void Write7Bit(uint value, Stream stream)
//        {
//            while (value > 127)
//            {
//                //Set bit 7 to true indicating more
//                stream.WriteByte((byte)(value | 128));
//                value = value >> 7;
//            }
//            stream.WriteByte((byte)value);
//        }
//        public static void Write7Bit(ulong value, Stream stream)
//        {
//            while (value > 127)
//            {
//                //Set bit 7 to true indicating more
//                stream.WriteByte((byte)(value | 128));
//                value = value >> 7;
//            }
//            stream.WriteByte((byte)value);
//        }

//        public unsafe static uint Read7BitUInt32(byte[] stream, int position, out int newPosition)
//        {
//            fixed (byte* lp = stream)
//            {
//                ulong value = *(ulong*)(lp + position);
//                uint b0 = (byte)value;
//                uint b1 = (byte)(value >> 8);
//                uint b2 = (byte)(value >> 16);
//                uint b3 = (byte)(value >> 24);
//                uint b4 = (byte)(value >> 32);

//                if (b0 < 128)
//                {
//                    newPosition = position + 1;
//                    return b0;
//                }
//                if (b1 < 128)
//                {
//                    newPosition = position + 2;
//                    return (b1<<7) | b0;
//                }
//                if (b2 < 128)
//                {
//                    newPosition = position + 3;
//                    return (b2<<14) | (b1 << 7) | b0;
//                }
//                if (b3 < 128)
//                {
//                    newPosition = position + 4;
//                    return (b3 << 21) | (b2 << 14) | (b1 << 7) | b0;
//                }
//                if (b4 < 128)
//                {
//                    newPosition = position + 5;
//                    return (b4 << 28) | (b3 << 21) | (b2 << 14) | (b1 << 7) | b0;
//                }
//                throw new Exception("Bad encoded int");
//            }

//        }

//        //public unsafe static uint Read7BitUInt32(byte[] stream, int position, out int newPosition)
//        //{
//        //    byte nextByte;
//        //    uint value = 0;
//        //    int bitCount = 0;
//        //    fixed (byte* lp = stream)
//        //    {
//        //        byte* lp2 = lp + position;

//        //        while (bitCount < 35)
//        //        {
//        //            nextByte = *lp2++;
//        //            value |= (uint)(nextByte & 127) << bitCount;
//        //            if ((nextByte & 128) == 0)
//        //            {
//        //                newPosition = (int)(lp2 - lp);
//        //                return value;
//        //            }
//        //            bitCount += 7;
//        //        }
//        //        throw new Exception("Bad encoded int");
//        //    }
//        //}

//        public static uint Read7BitUInt32(Stream stream)
//        {
//            byte nextByte;
//            uint value = 0;
//            int bitCount = 0;
//            while (bitCount < 35)
//            {
//                nextByte = (byte)stream.ReadByte();
//                value |= (uint)(nextByte & 127) << bitCount;
//                if ((nextByte & 128) == 0)
//                    return value;
//                bitCount += 7;
//            }
//            throw new Exception("Bad encoded int");
//        }
//        public static ulong Read7BitUInt64(Stream stream)
//        {
//            byte nextByte;
//            ulong value = 0;
//            int bitCount = 0;
//            while (bitCount < 70)
//            {
//                nextByte = (byte)stream.ReadByte();
//                value |= (ulong)(nextByte & 127) << bitCount;
//                if ((nextByte & 128) == 0)
//                    return value;
//                bitCount += 7;
//            }
//            throw new Exception("Bad encoded long");
//        }
//    }
//}

