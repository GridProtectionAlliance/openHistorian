//using System;
//using System.IO;

//namespace openHistorian.Core
//{
//    public class CompressBlockUIn32
//    {
//        public static unsafe int Initialize(byte[] buffer, int position)
//        {
//            m_BytePosition = 0;
//            m_oldValue = 0;
//            m_reservation1 = position;
//            m_reservation2 = position + 8;
//            return position + 16;
//        }
//        public static int AddValue(uint prev, uint current, byte[] buffer, int position)
//        {
//            int pos = Compress(buffer, position, m_oldValue, value);
//            m_oldValue = value;
//            return pos;
//        }
//        static int Compress(byte[] buffer, int position, uint previousValue, uint currentValue)
//        {
//            uint diff = previousValue ^ currentValue;
//            Write7Bit(diff);
//            if (m_BytePosition > 8)
//                return FlushItem(buffer, position);
//            else
//                return position;
//        }
//        public static unsafe void Flush(byte[] buffer)
//        {
//            fixed (byte* lp = buffer)
//            {
//                long* ptr = (long*)(lp + m_reservation1);
//                fixed (byte* lp2 = m_data)
//                {
//                    long* ptr2 = (long*)(lp2);
//                    *ptr = *ptr2;
//                    ptr2[0] = ptr2[1];
//                }
//            }
//        }
//        static unsafe int FlushItem(byte[] buffer, int position)
//        {
//            fixed (byte* lp = buffer)
//            {
//                long* ptr = (long*)(lp + m_reservation1);
//                fixed (byte* lp2 = m_data)
//                {
//                    long* ptr2 = (long*)(lp2);
//                    *ptr = *ptr2;
//                    ptr2[0] = ptr2[1];
//                }
//            }
//            m_reservation1 = m_reservation2;
//            m_reservation2 = position;
//            m_BytePosition -= 8;
//            return position + 8;
//        }
//        public static void Write7Bit(uint value)
//        {
//            while (value > 127)
//            {
//                //Set bit 7 to true indicating more
//                WriteByte((byte)(value | 128));
//                value = value >> 7;
//            }
//            WriteByte((byte)(value));
//        }
//        public static unsafe void WriteByte(byte b)
//        {
//            fixed (byte* lp = m_data)
//            {
//                lp[m_BytePosition] = b;
//            }
//            m_BytePosition++;
//        }
//    }
//}
