using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.Unmanaged.Generic
{
    public class BPlusTree<TKey, TValue> : BPlusTreeBase<TKey, TValue>
        where TKey : struct, IValueType<TKey>
        where TValue : struct, IValueType<TValue>
    {

        public BPlusTree(BinaryStream leafNodeStream, BinaryStream internNodeStream)
            : base(leafNodeStream, internNodeStream)
        {

        }
        public BPlusTree(BinaryStream leafNodeStream, BinaryStream internNodeStream, int blockSize)
            : base(leafNodeStream, internNodeStream, blockSize)
        {

        }
        public BPlusTree(BinaryStream stream)
            : base(stream)
        {
        }

        public BPlusTree(BinaryStream stream, int blockSize)
            : base(stream, blockSize)
        {
        }

        protected override void SaveValue(TValue value, BinaryStream stream)
        {
            value.SaveValue(stream);
        }

        protected override TValue LoadValue(BinaryStream stream)
        {
            TValue value = default(TValue);
            value.LoadValue(stream);
            return value;
        }

        protected override int SizeOfValue()
        {
            return default(TValue).SizeOf;
        }

        protected override int SizeOfKey()
        {
            return default(TKey).SizeOf;
        }

        protected override void SaveKey(TKey value, BinaryStream stream)
        {
            value.SaveValue(stream);
        }

        protected override TKey LoadKey(BinaryStream stream)
        {
            TKey value = default(TKey);
            value.LoadValue(stream);
            return value;
        }

        protected override int CompareKeys(TKey first, TKey last)
        {
            return first.CompareTo(last);
        }

        protected override int CompareKeys(TKey first, BinaryStream stream)
        {
            return first.CompareToStream(stream);
        }
    }
}
