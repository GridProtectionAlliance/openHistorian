//using System;
//using System.IO;

//namespace openHistorian.Core
//{
//    public unsafe class CompressBlockUIn32_2
//    {
//        public static int AddValue(uint value1, uint value2, uint value3, uint value4, byte[] buffer, int position)
//        {
//            fixed (byte* lp = buffer)
//            {
//                byte* lp2 = lp + 1 + position;
//                while (value1 > 0)
//                {
//                    *lp2 = (byte)(value1 & 0xFF);
//                    lp2++;
//                    value1 = value1 >> 8;
//                }
//                while (value2 > 0)
//                {
//                    *lp2 = (byte)(value2 & 0xFF);
//                    lp2++;
//                    value2 = value2 >> 8;
//                }
//                while (value3 > 0)
//                {
//                    *lp2 = (byte)(value3 & 0xFF);
//                    lp2++;
//                    value3 = value3 >> 8;
//                }
//                while (value4 > 0)
//                {
//                    *lp2 = (byte)(value4 & 0xFF);
//                    lp2++;
//                    value4 = value4 >> 8;
//                }
//                return (int)(lp2 - lp);
//            }

//        }
//        //public static int AddValue(uint value1, uint value2, uint value3, uint value4, byte[] buffer, int position)
//        //{
//        //    int pos = position;
//        //    int size1 = Write(value1, buffer, pos += 1);
//        //    int size2 = Write(value1, buffer, pos += size1 + 1);
//        //    int size3 = Write(value1, buffer, pos += size2 + 1);
//        //    int size4 = Write(value1, buffer, pos += size3 + 1);
//        //    buffer[position] = (byte)(size1 | (size2 << 2) | (size3 << 4) | (size4 << 6));
//        //    return pos + size4 + 1;
//        //}

//        static int Write(uint value, byte[] buffer, int position)
//        {
//            fixed (byte* lp = buffer)
//            {
//                byte* lp2 = lp + position;
//                while (value > 0)
//                {
//                    //*lp2 = (byte)(value & 0xFF);
//                    lp2++;
//                    value = value >> 8;
//                }
//                return (int)(lp2 - lp) - position - 1;
//            }
//        }


//        //static int Write(uint value, byte[] buffer, int position)
//        //{
//        //    fixed (byte* lp = buffer)
//        //    {
//        //        byte* lp2 = lp + position;
//        //        if (value <= 0xFF)
//        //        {
//        //            //*lp2 = (byte)value;
//        //            return 0;
//        //        }
//        //        else if (value <= 0xFFFF)
//        //        {
//        //            //*(ushort*)lp2 = (ushort)value;
//        //            return 1;
//        //        }
//        //        else if (value <= 0xFFFFFF)
//        //        {
//        //            //*(uint*)lp2 = (uint)value;
//        //            return 2;
//        //        }
//        //        else
//        //        {
//        //            //*(uint*)lp2 = (uint)value;
//        //            return 3;
//        //        }
//        //    }
//        //}

//    }
//}
