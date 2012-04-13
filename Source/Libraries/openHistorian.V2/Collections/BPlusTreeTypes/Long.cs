using openHistorian.V2.IO;

namespace openHistorian.V2.Collections.BPlusTreeTypes
{
    struct Long : IBPlusTreeType<Long>
    {
        //public TreeTypeLong()
        //{

        //}
        public Long(long value)
        {
            Value = value;
        }

        public long Value;
        public int SizeOf
        {
            get
            {
                return 8;
            }
        }

        public void LoadValue(IBinaryStream stream)
        {
            Value = stream.ReadInt64();
        }

        public void SaveValue(IBinaryStream stream)
        {
            stream.Write(Value);
        }
        public int CompareToStream(IBinaryStream stream)
        {
            return Value.CompareTo(stream.ReadInt64());
        }
        public int CompareTo(Long key)
        {
            return Value.CompareTo(key.Value);
        }

        public static implicit operator Long(long value)
        {
            return new Long(value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

}
