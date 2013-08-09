//using System;
//using System.IO;

//namespace openHistorian.Core
//{
//    public unsafe class CompressBlockUIn32
//    {
//        int m_reservation;
//        int m_bitPosition;
//        int m_meta;

//        public void Initialize()
//        {
//            m_bitPosition = 8;
//            m_reservation = -1;
//        }
//        public int AddValue(uint value, uint oldValue, byte[] buffer, int position)
//        {
//            if (m_bitPosition == 8)
//            {
//                if (m_reservation > 0)
//                    buffer[m_reservation] = (byte)m_meta;
//                m_meta = 0;
//                m_reservation = position;
//                position++;
//                m_bitPosition = 0;
//            }
//            int pos = Compress(buffer, position, oldValue, value);
//            return pos;
//        }

//        public void Flush(byte[] buffer)
//        {
//            if (m_reservation > 0)
//                buffer[m_reservation] = (byte)m_meta;
//            m_reservation = -1;
//            m_meta = 0;
//            m_bitPosition = 8;
//        }

//        int Compress(byte[] buffer, int position, uint previousValue, uint currentValue)
//        {
//            uint diff = previousValue ^ currentValue;
//            return Write(diff,buffer,position);
//        }

//        int Write(uint value, byte[] buffer, int position)
//        {
//            int count = 0;
//            fixed (byte* lp = buffer)
//            {
//                byte* lp2 = lp;
//                lp2 += position;
//                do
//                {
//                    *lp2 = (byte)(value);
//                    lp2++;
//                    count++;
//                    value = value >> 8;
//                } while (value > 255);
//                WriteLength((byte)(count - 1));
//                return position + count;
//            }
//        }

//        //int Write(uint value, byte[] buffer, int position)
//        //{
//        //    if (value <= 0xFF)
//        //    {
//        //        WriteLength(1-1);
//        //        fixed (byte* lp = buffer)
//        //        {
//        //            lp[position] = (byte)value;
//        //        }
//        //        return position + 1;
//        //    }
//        //    if (value <= 0xFFFF)
//        //    {
//        //        WriteLength(2-1);
//        //        fixed (byte* lp = buffer)
//        //        {
//        //            *(ushort*)(lp + position) = (ushort)value;
//        //        }
//        //        return position + 2;
//        //    }
//        //    if (value <= 0xFFFFFF)
//        //    {
//        //        WriteLength(3-1);
//        //        fixed (byte* lp = buffer)
//        //        {
//        //            *(uint*)(lp + position) = (uint)value;
//        //        }
//        //        return position + 3;
//        //    }
//        //    WriteLength(4-1);
//        //    fixed (byte* lp = buffer)
//        //    {
//        //        *(uint*)(lp + position) = (uint)value;
//        //    }
//        //    return position + 4;
//        //}

//        void WriteLength(byte b)
//        {
//            m_meta |= (b << m_bitPosition);
//            m_bitPosition += 2;
//        }

//    }
//}

