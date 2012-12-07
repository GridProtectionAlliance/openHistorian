//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TVA;
//using openHistorian.Collections.BPlusTreeTypes;

//namespace openHistorian
//{
//    public class DataPoint : IDataPoint
//    {
//        int m_historianId;
//        Ticks m_timeTag;
//        uint m_flags;
//        float m_value;

//        public DataPoint(DateTimeLong key, IntegerFloat value)
//        {
//            m_historianId = (int)key.Key;
//            m_timeTag = key.Time;
//            m_flags = (uint)value.Value1;
//            m_value = value.Value2;
//        }


//        public int ParseBinaryImage(byte[] buffer, int startIndex, int length)
//        {
//            throw new NotImplementedException();
//        }

//        public int GenerateBinaryImage(byte[] buffer, int startIndex)
//        {
//            throw new NotImplementedException();
//        }

//        public int BinaryLength
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public int CompareTo(object obj)
//        {
//            if (obj is IDataPoint)
//            {
//                return CompareTo((IDataPoint)obj);
//            }
//            return 0;
//        }
//        int CompareTo(IDataPoint other)
//        {
//            int result = HistorianID.CompareTo(other.HistorianID);
//            if (result != 0)
//                return result;
//            else
//                return Time.CompareTo(other.Time);
//        }

//        public string ToString(string format, IFormatProvider formatProvider)
//        {
//            return m_historianId.ToString(formatProvider);
//        }

//        public int HistorianID
//        {
//            get
//            {
//                return m_historianId;
//            }
//            set
//            {
//                m_historianId = value;
//            }
//        }

//        public Ticks Time
//        {
//            get
//            {
//                return m_timeTag;
//            }
//            set
//            {
//                m_timeTag = value;
//            }
//        }

//        public float Value
//        {
//            get
//            {
//                return m_value;
//            }
//            set
//            {
//                m_value = value;
//            }
//        }

//        public uint Flags
//        {
//            get
//            {
//                return m_flags;
//            }
//            set
//            {
//                m_flags = value;
//            }
//        }
//    }
//}
