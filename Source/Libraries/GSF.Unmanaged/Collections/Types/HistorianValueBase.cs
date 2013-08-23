using GSF.IO;

namespace openHistorian.Collections
{
    public abstract class HistorianValueBase<TValue>
    {
        public abstract void CopyTo(TValue other);
        public abstract void Write(BinaryStreamBase stream);
        public abstract void Read(BinaryStreamBase stream);
        public abstract void WriteCompressed(BinaryStreamBase stream, TValue previousValue);
        public abstract void ReadCompressed(BinaryStreamBase stream, TValue previousValue);
        public abstract void Clear();
    }
}