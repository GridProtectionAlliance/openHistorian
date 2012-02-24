//using System;
//using System.IO;

//namespace Historian
//{
//    public unsafe struct CompressBlockUIn32
//    {
//        int m_reservation1;
//        int m_reservation2;
//        ulong m_firstBin;
//        ulong m_secondBin;
//        int m_bitPosition;
//        uint m_oldValue;
//        public int Initialize(byte[] buffer, int position)
//        {
//            m_firstBin = 0;
//            m_secondBin = 0;
//            m_bitPosition = 0;
//            m_oldValue = 0;
//            m_reservation1 = position;
//            m_reservation2 = position + 8;
//            return position + 16;
//        }

//        public int AddValue(uint value, byte[] buffer, int position)
//        {
//            int pos = Compress(buffer, position, m_oldValue, value);
//            m_oldValue = value;
//            return pos;
//        }

//        int Compress(byte[] buffer, int position, uint previousValue, uint currentValue)
//        {
//            uint diff = previousValue ^ currentValue;
//            Write7Bit(diff);
//            if (m_bitPosition > 64)
//                return FlushItem(buffer, position);
//            else
//                return position;
//        }

//        public unsafe void Flush(byte[] buffer)
//        {
//            fixed (byte* lp = buffer)
//            {
//                ulong* ptr = (ulong*)(lp + m_reservation1);
//                *ptr = m_firstBin;
//            }
//        }

//        unsafe int FlushItem(byte[] buffer, int position)
//        {
//            fixed (byte* lp = buffer)
//            {
//                ulong* ptr = (ulong*)(lp + m_reservation1);
//                *ptr = m_firstBin;
//            }
//            m_firstBin = m_secondBin;
//            m_secondBin = 0L;
//            m_reservation1 = m_reservation2;
//            m_reservation2 = position;
//            m_bitPosition -= 64;
//            return position + 8;
//        }

//        public void Write7Bit(uint value)
//        {
//            while (value > 127)
//            {
//                //Set bit 7 to true indicating more
//                WriteByte((byte)(value | 128));
//                value = value >> 7;
//            }
//             WriteByte((byte)(value));
//        }

//        public void WriteByte(byte b)
//        {
//            if (m_bitPosition < 64)
//            {
//                m_firstBin |= (ulong)b << m_bitPosition;
//            }
//            else if (m_bitPosition < 128)
//            {
//                m_secondBin |= (ulong)b << (m_bitPosition-64);
//            }
//            else
//            {
//                throw new Exception("Error");
//            }
//            m_bitPosition += 8;
//        }
//    }
//}
