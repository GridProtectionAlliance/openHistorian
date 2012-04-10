//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.Core.Unmanaged.Generic
//{
//    public class BPlusTree2LongLong : BPlusTreeBase
//    {
//        long m_value1 = default (long);
//        long m_key1 = default(long);
//        long m_key2 = default(long);
//        long m_key3 = default(long);

//        public BPlusTree2LongLong(BinaryStream stream) : base(stream)
//        {
//        }

//        public BPlusTree2LongLong(BinaryStream stream, int blockSize)
//            : base(stream, blockSize)
//        {
//        }

//        protected override int SizeOfValue()
//        {
//            return 8;
//        }

//        protected override int SizeOfKey()
//        {
//            return 8;
//        }

//        protected override void SaveValue1()
//        {
//            m_stream.Write(m_value1);
//        }

//        protected override void LoadValue1()
//        {
//            m_value1 = m_stream.ReadInt64();
//        }

//        protected override void SaveKey1()
//        {
//            m_stream.Write(m_key1);
//        }

//        protected override void LoadKey1()
//        {
//            m_key1 = m_stream.ReadInt64();
//        }

//        protected override void SaveKey2()
//        {
//            m_stream.Write(m_key2);
//        }

//        protected override void LoadKey2()
//        {
//            m_key2 = m_stream.ReadInt64();
//        }

//        protected override void SaveKey3()
//        {
//            m_stream.Write(m_key3);
//        }

//        protected override void LoadKey3()
//        {
//            m_key3 = m_stream.ReadInt64();
//        }

//        protected override int CompareKeys12()
//        {
//            return m_key1.CompareTo(m_key2);
//        }

//        protected override int CompareKeys23()
//        {
//            return m_key2.CompareTo(m_key3);
//        }

//        protected override void CopyKey3ToKey2()
//        {
//            m_key2 = m_key3;
//        }

//        protected override int CompareKey1WithStream()
//        {
//            return m_key1.CompareTo(m_stream.ReadInt64());
//        }

//        public void AddData(long key, long value)
//        {
//            m_key1 = key;
//            m_value1 = value;
//            AddData();
//        }

//        public long GetData(long key)
//        {
//            m_key1 = key;
//            GetData();
//            return m_value1;
//        }


//        unsafe protected override bool LeafNodeSeekToKey(out int offset)
//        {
//            long key = m_key1;
//            int leafStructureSize = m_leafStructureSize;
//            long startAddress = m_currentNode * m_blockSize + NodeHeader.Size;
//            m_stream.Position = startAddress;

//            byte* pos;
//            byte* start;
//            int currentIndex;
//            int length;
//            m_stream.GetRawDataBlock(false, out start, out currentIndex, out length);
//            if (length < m_blockSize - NodeHeader.Size)
//                throw new Exception();

//            start += currentIndex;

//            int min = 0;
//            int max = m_childCount - 1;

//            while (min <= max)
//            {
//                int mid = min + (max - min >> 1);
//                pos = start + leafStructureSize * mid;

//                //int tmpKey = LeafNodeCompareKeys(key, m_stream);
//                if (key == *(long*)pos)
//                {
//                    offset = NodeHeader.Size + leafStructureSize * mid;
//                    return true;
//                }
//                if (key > *(long*)pos)
//                    min = mid + 1;
//                else
//                    max = mid - 1;
//            }
//            offset = NodeHeader.Size + leafStructureSize * min;
//            return false;
//        }

//        unsafe protected override bool InternalNodeSeekToKey(out int offset)
//        {
//            long key = m_key1;

//            int internalStructureSize = m_internalNodeStructureSize;
//            long startAddress = m_internalNodeCurrentNode * m_blockSize + NodeHeader.Size + sizeof(uint);

//            m_stream.Position = startAddress;

//            byte* pos;
//            byte* start;
//            int currentIndex;
//            int length;
//            m_stream.GetRawDataBlock(false, out start, out currentIndex, out length);
//            if (length < m_blockSize - NodeHeader.Size - sizeof(uint))
//                throw new Exception();

//            start += currentIndex;

//            int min = 0;
//            int max = m_internalNodeChildCount - 1;

//            while (min <= max)
//            {
//                int mid = min + (max - min >> 1);
//                pos = start + internalStructureSize * mid;

//                if (key == *(long*)pos)
//                {
//                    offset = NodeHeader.Size + sizeof(uint) + internalStructureSize * mid;
//                    return true;
//                }
//                if (key > *(long*)pos)
//                    min = mid + 1;
//                else
//                    max = mid - 1;
//            }

//            offset = NodeHeader.Size + sizeof(uint) + internalStructureSize * min;
//            return false;
//        }

//    }
//}
