using System;
using GSF.IO;

namespace openHistorian.Collections
{
    public abstract class HistorianKeyBase<TKey>
        : IComparable<TKey>
    {
        /// <summary>
        /// The timestamp stored as native ticks. 
        /// </summary>
        public ulong Timestamp;

        /// <summary>
        /// The id number of the point.
        /// </summary>
        public ulong PointID;

        public abstract void CopyTo(TKey other);
        public abstract void SetMin();
        public abstract void SetMax();
        public abstract void Write(BinaryStreamBase stream);
        public abstract void Read(BinaryStreamBase stream);
        public abstract void WriteCompressed(BinaryStreamBase stream, TKey previousKey);
        public abstract void ReadCompressed(BinaryStreamBase stream, TKey previousKey);
        public abstract void Clear();

        public abstract int CompareTo(TKey other);
        public bool IsLessThan(TKey other)
        {
            return CompareTo(other) < 0;
        }
        public bool IsLessThanOrEqualTo(TKey other)
        {
            return CompareTo(other) <= 0;
        }
        public bool IsGreaterThan(TKey other)
        {
            return CompareTo(other) > 0;
        }
        public bool IsGreaterThanOrEqualTo(TKey other)
        {
            return CompareTo(other) >= 0;
        }
        public bool IsEqualTo(TKey other)
        {
            return CompareTo(other) == 0;
        }
    }
}