using System;
using System.Runtime.InteropServices;
using openHistorian.V2.IO;

namespace openHistorian.V2.Collections.BPlusTreeTypes
{
    [StructLayout(LayoutKind.Explicit)]
    public struct DateTimeLong : IBPlusTreeType<DateTimeLong>
    {
        [FieldOffset(0)]
        public DateTime Time;
        [FieldOffset(8)]
        public long Key;

        public int SizeOf
        {
            get
            {
                return 16;
            }
        }

        public void LoadValue(IBinaryStream stream)
        {
            Time = stream.ReadDateTime();
            Key = stream.ReadInt64();
        }

        public void SaveValue(IBinaryStream stream)
        {
            stream.Write(Time.Ticks);
            stream.Write(Key);
        }

        public int CompareToStream(IBinaryStream stream)
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
        public int CompareTo(DateTimeLong key)
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
