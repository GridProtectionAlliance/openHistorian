using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.V2.Unmanaged.Generic
{
    public class BPlusTreeLongLong : BPlusTreeBase<long,long> 
    {

        public BPlusTreeLongLong(BinaryStream stream) : base(stream)
        {
        }

        public BPlusTreeLongLong(BinaryStream stream, int blockSize)
            : base(stream, blockSize)
        {
        }

        protected override void SaveValue(long value, BinaryStream stream)
        {
            stream.Write(value);
        }

        protected override long LoadValue(BinaryStream stream)
        {
            return stream.ReadInt64();
        }

        protected override int SizeOfValue()
        {
            return 8;
        }

        protected override int SizeOfKey()
        {
            return 8;
        }

        protected override void SaveKey(long value, BinaryStream stream)
        {
            stream.Write(value);
        }

        protected override long LoadKey(BinaryStream stream)
        {
            return stream.ReadInt64();
        }

        protected override int CompareKeys(long first, long last)
        {
            return first.CompareTo(last);
        }

        protected override int CompareKeys(long first, BinaryStream stream)
        {
            return first.CompareTo(stream.ReadInt64());
        }

        unsafe protected override bool LeafNodeSeekToKey(long key, out int offset)
        {
            int leafStructureSize = m_leafStructureSize;
            long startAddress = m_currentNode * m_blockSize + NodeHeader.Size;
            m_leafNodeStream.Position = startAddress;

            byte* pos;
            byte* start;
            int currentIndex;
            int length;
            m_leafNodeStream.GetRawDataBlock(false, out start, out currentIndex, out length);
            if (length<m_blockSize-NodeHeader.Size)
                throw new Exception();

            start += currentIndex;

            int min = 0;
            int max = m_childCount - 1;

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                pos = start + leafStructureSize * mid;

                //int tmpKey = LeafNodeCompareKeys(key, m_stream);
                if (key == *(long*)pos)
                {
                    offset = NodeHeader.Size + leafStructureSize * mid;
                    return true;
                }
                if (key > *(long*)pos)
                    min = mid + 1;
                else
                    max = mid - 1;
            }
            offset = NodeHeader.Size + leafStructureSize * min;
            return false;
        }

        unsafe protected override bool InternalNodeSeekToKey(long key, out int offset)
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

            while (min <= max)
            {
                int mid = min + (max - min >> 1);
                pos = start + internalStructureSize * mid;

                if (key == *(long*)pos)
                {
                    offset = NodeHeader.Size + sizeof(uint) + internalStructureSize * mid;
                    return true;
                }
                if (key > *(long*)pos)
                    min = mid + 1;
                else
                    max = mid - 1;
            }

            offset = NodeHeader.Size + sizeof(uint) + internalStructureSize * min;
            return false;
        }

    }
}
