using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace openHistorian.V2.StorageSystem.Specialized.TimeKeyPair
{
    [StructLayout(LayoutKind.Explicit)]
    struct KeyType : IKeyType<KeyType>, IValueType<KeyType>
    {
        [FieldOffset(0)]
        public DateTime Time;
        [FieldOffset(8)]
        public long Key;

        public ITreeLeafNodeMethods<KeyType> GetLeafNodeMethods()
        {
            return new LeafNodeMethods<KeyType>();
        }

        public ITreeInternalNodeMethods<KeyType> GetInternalNodeMethods()
        {
            return new InternalNodeMethods<KeyType>();
        }

        public int SizeOf
        {
            get
            {
                return 16;
            }
        }

        public void LoadValue(BinaryStream stream)
        {
            Time = stream.ReadDateTime();
            Key = stream.ReadInt64();
        }

        public void SaveValue(BinaryStream stream)
        {
            stream.Write(Time.Ticks);
            stream.Write(Key);
        }

        public int CompareToStream(BinaryStream stream)
        {
            DateTime time = stream.ReadDateTime();
            long key = stream.ReadInt64();

            if (Time.Ticks == time.Ticks && Key == key)
                return 0;
            if (Time.Ticks > time.Ticks)
                return 1;
            if (Time.Ticks < time.Ticks)
                return -1;
            if (Key > key)
                return 1;
            return -1;

        }
        public int CompareTo(KeyType key)
        {
            if (Time.Ticks == key.Time.Ticks && Key == key.Key)
                return 0;
            if (Time.Ticks > key.Time.Ticks)
                return 1;
            if (Time.Ticks < key.Time.Ticks)
                return -1;
            if (Key > key.Key)
                return 1;
            return -1;
        }
    }
}
