using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.V2.StorageSystem.Generic
{
    struct TreeTypeLong : ITreeType<TreeTypeLong>
    {
        //public TreeTypeLong()
        //{
            
        //}
        public TreeTypeLong(long value)
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

        public void LoadValue(BinaryStream stream)
        {
            Value = stream.ReadInt64();
        }

        public void SaveValue(BinaryStream stream)
        {
            stream.Write(Value);
        }
        public int CompareToStream(BinaryStream stream)
        {
            return Value.CompareTo(stream.ReadInt64());
        }
        public int CompareTo(TreeTypeLong key)
        {
            return Value.CompareTo(key.Value);
        }

        public static implicit operator TreeTypeLong(long value)
        {
            return new TreeTypeLong(value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

}
