using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.Unmanaged.Generic
{
    public class BPlusTree<TKey, TValue> : BPlusTreeBase<TKey,TValue> 
        where TKey : struct, IValueType<TKey>
        where TValue : struct, IValueType<TValue>
        
    {
        public BPlusTree(BinaryStream stream) : base(stream)
        {
        }

        public BPlusTree(BinaryStream stream, int blockSize) : base(stream, blockSize)
        {
        }

        public override void SaveValue(TValue value, BinaryStream stream)
        {
            value.SaveValue(stream);
        }

        public override TValue LoadValue(BinaryStream stream)
        {
            TValue value = default(TValue);
            value.LoadValue(stream);
            return value;
        }

        public override int SizeOfValue()
        {
            return default(TValue).SizeOf;
        }

        public override int SizeOfKey()
        {
            return default(TKey).SizeOf;
        }

        public override void SaveKey(TKey value, BinaryStream stream)
        {
            value.SaveValue(stream);
        }

        public override TKey LoadKey(BinaryStream stream)
        {
            TKey value = default(TKey);
            value.LoadValue(stream);
            return value;
        }

        public override int CompareKeys(TKey first, TKey last)
        {
            return first.CompareTo(last);
        }

        public override int CompareKeys(TKey first, BinaryStream stream)
        {
            return first.CompareToStream(stream);
        }
    }
}
