//using System;
//using System.IO;

//namespace Historian
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
//            return Write7Bit((uint)diff, stream,position);
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
