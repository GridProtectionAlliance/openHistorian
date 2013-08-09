//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.Core.PointStream
//{
//    public class SinglePointMessage
//    {
//        PointWriter m_Writer;
//        int m_Count;
//        internal void SetWriter(PointWriter writer)
//        {
//            m_Writer = writer;
//            m_Count = writer.PositionEndOfData;
//        }
//        void DecrementRead()
//        {
//            m_Count--;
//            if (m_Count < 0)
//                throw new Exception("Writing Past The End Of The Stream Was Detected");
//        }
//        public void Write(byte value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//        public void Write(short value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//        public void Write(int value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//        public void Write(long value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//        public void Write(ushort value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//        public void Write(uint value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//        public void Write(ulong value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//        public void Write(decimal value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//        public void Write(Guid value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//        public void Write(DateTime value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//        public void Write(float value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//        public void Write(double value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//        public void Write(bool value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//        public void Write(string value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//        public void Write(byte[] value, int offset, int count)
//        {
//            DecrementRead();
//            m_Writer.Write(value, offset, count);
//        }
//        public void Write(byte[] value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//        public void Write(sbyte value)
//        {
//            DecrementRead();
//            m_Writer.Write(value);
//        }
//    }
//}

