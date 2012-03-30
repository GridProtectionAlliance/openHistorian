using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using openHistorian.Core.Unmanaged.Generic.TimeKeyPair;

namespace openHistorian.Core.Unmanaged.Generic
{
    public class BPlusTreeTSD : BPlusTreeBase<KeyType, TreeTypeIntFloat>
    {

        public BPlusTreeTSD(BinaryStream leafNodeStream, BinaryStream internNodeStream)
            : base(leafNodeStream, internNodeStream)
        {

        }
        public BPlusTreeTSD(BinaryStream leafNodeStream, BinaryStream internNodeStream, int blockSize)
            : base(leafNodeStream, internNodeStream, blockSize)
        {

        }

        public BPlusTreeTSD(BinaryStream stream)
            : base(stream)
        {
        }

        public BPlusTreeTSD(BinaryStream stream, int blockSize)
            : base(stream, blockSize)
        {
        }

        protected override void SaveValue(TreeTypeIntFloat value, BinaryStream stream)
        {
            value.SaveValue(stream);
        }

        protected override TreeTypeIntFloat LoadValue(BinaryStream stream)
        {
            TreeTypeIntFloat value = default(TreeTypeIntFloat);
            value.LoadValue(stream);
            return value;
        }

        protected override int SizeOfValue()
        {
            return 8;
        }

        protected override int SizeOfKey()
        {
            return 16;
        }

        protected override void SaveKey(KeyType value, BinaryStream stream)
        {
            value.SaveValue(stream);
        }

        protected override KeyType LoadKey(BinaryStream stream)
        {
            KeyType value = default(KeyType);
            value.LoadValue(stream);
            return value;
        }

        protected override int CompareKeys(KeyType first, KeyType last)
        {
            return first.CompareTo(last);
        }

        protected override int CompareKeys(KeyType first, BinaryStream stream)
        {
            return first.CompareToStream(stream);
        }

        unsafe protected override bool LeafNodeSeekToKey(KeyType key, out int offset)
        {
            int leafStructureSize = m_leafStructureSize;
            long startAddress = m_currentNode * m_blockSize + NodeHeader.Size;
            m_leafNodeStream.Position = startAddress;

            byte* pos;
            byte* start;
            int currentIndex;
            int length;
            m_leafNodeStream.GetRawDataBlock(false, out start, out currentIndex, out length);
            if (length < m_blockSize - NodeHeader.Size)
                throw new Exception();

            start += currentIndex;

            int min = 0;
            int max = m_childCount - 1;

            long key1 = key.Time.Ticks;
            long key2 = key.Key;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                pos = start + leafStructureSize * mid;

                //int tmpKey = LeafNodeCompareKeys(key, m_stream);
                if (key1 == *(long*)pos && key2 == *(long*)(pos+8))
                {
                    offset = NodeHeader.Size + leafStructureSize * mid;
                    return true;
                }
                if (key1 > *(long*)pos || (key1 == *(long*)pos && key2 > *(long*)(pos + 8)))
                    min = mid + 1;
                else
                    max = mid - 1;
            }
            offset = NodeHeader.Size + leafStructureSize * min;
            return false;
        }

        unsafe protected override bool InternalNodeSeekToKey(KeyType key, out int offset)
        {
            int internalStructureSize = m_internalNodeStructureSize;
            long startAddress = m_internalNodeCurrentNode * m_blockSize + NodeHeader.Size + sizeof(uint);

            m_internalNodeStream.Position = startAddress;

            byte* pos;
            byte* start;
            int currentIndex;
            int length;
            m_internalNodeStream.GetRawDataBlock(false, out start, out currentIndex, out length);
            if (length < m_blockSize - NodeHeader.Size - sizeof(uint))
                throw new Exception();

            start += currentIndex;

            int min = 0;
            int max = m_internalNodeChildCount - 1;

            long key1 = key.Time.Ticks;
            long key2 = key.Key;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                pos = start + internalStructureSize * mid;

                if (key1 == *(long*)pos && key2 == *(long*)(pos + 8))
                {
                    offset = NodeHeader.Size + sizeof(uint) + internalStructureSize * mid;
                    return true;
                }
                if (key1 > *(long*)pos || (key1 == *(long*)pos && key2 > *(long*)(pos + 8)))
                    min = mid + 1;
                else
                    max = mid - 1;
            }

            offset = NodeHeader.Size + sizeof(uint) + internalStructureSize * min;
            return false;
        }

    }
}
