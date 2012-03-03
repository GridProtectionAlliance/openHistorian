using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace openHistorian.Core.StorageSystem.Specialized.TimeKeyPair
{
    [StructLayout(LayoutKind.Explicit)]
    struct KeyType : IKeyType<KeyType>
    {
        [FieldOffset(0)]
        public DateTime Time;
        [FieldOffset(8)]
        public long Key;

        public ITreeLeafNodeMethods<KeyType> GetLeafNodeMethods()
        {
            return new LeafNodeMethods();
        }

        public ITreeInternalNodeMethods<KeyType> GetInternalNodeMethods()
        {
            return new InternalNodeMethods();
        }

        public static bool operator >(KeyType a, KeyType b)
        {
            return (a.Time > b.Time) || (a.Time == b.Time && a.Key > b.Key);
        }
        public static bool operator <(KeyType a, KeyType b)
        {
            return (a.Time < b.Time) || (a.Time == b.Time && a.Key < b.Key);
        }
        public static bool operator >=(KeyType a, KeyType b)
        {
            return (a.Time >= b.Time) || (a.Time == b.Time && a.Key >= b.Key);
        }
        public static bool operator <=(KeyType a, KeyType b)
        {
            return (a.Time <= b.Time) || (a.Time == b.Time && a.Key <= b.Key);
        }
        public static bool operator ==(KeyType a, KeyType b)
        {
            return (a.Time == b.Time && a.Key == b.Key);
        }
        public static bool operator !=(KeyType a, KeyType b)
        {
            return (a.Time != b.Time || a.Key != b.Key);
        }
    }
}
