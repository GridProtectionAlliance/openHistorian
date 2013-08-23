using GSF.IO;

namespace openHistorian.Collections
{
    public abstract class HistorianKeyBase<TKey>
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
    }
}