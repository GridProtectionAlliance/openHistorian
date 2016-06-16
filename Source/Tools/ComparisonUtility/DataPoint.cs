using System;
using GSF;

namespace ComparisonUtility
{
    public class DataPoint
    {
        public ulong m_timestamp;

        public ulong Timestamp
        {
            get
            {
                return m_timestamp / Ticks.PerMillisecond * Ticks.PerMillisecond;
            }
            set
            {
                m_timestamp = value / Ticks.PerMillisecond * Ticks.PerMillisecond;
            }
        }

        public float ValueAsSingle
        {
            get
            {
                return BitMath.ConvertToSingle(Value);
            }
            set
            {
                Value = BitMath.ConvertToUInt64(value);
            }
        }

        public ulong PointID;
        public ulong Value;
        public ulong Flags;

        public void Clone(DataPoint destination)
        {
            destination.m_timestamp = m_timestamp;
            destination.PointID = PointID;
            destination.Value = Value;
            destination.Flags = Flags;
        }
    }
}