//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace openHistorian.Core.Unmanaged.Generic
//{
//    public class BPlusTree2<TKey, TValue> : BPlusTreeBase
//        where TKey : struct, IValueType<TKey>
//        where TValue : struct, IValueType<TValue>

//    {
//        TValue m_value1 = default (TValue);
//        TKey m_key1 = default(TKey);
//        TKey m_key2 = default(TKey);
//        TKey m_key3 = default(TKey);

//        public BPlusTree2(BinaryStream stream) : base(stream)
//        {
//        }

//        public BPlusTree2(BinaryStream stream, int blockSize) : base(stream, blockSize)
//        {
//        }

//        protected override int SizeOfValue()
//        {
//            return default(TValue).SizeOf;
//        }

//        protected override int SizeOfKey()
//        {
//            return default(TKey).SizeOf;
//        }

//        protected override void SaveValue1()
//        {
//            m_value1.SaveValue(m_stream);
//        }

//        protected override void LoadValue1()
//        {
//            m_value1.LoadValue(m_stream);
//        }

//        protected override void SaveKey1()
//        {
//            m_key1.SaveValue(m_stream);
//        }

//        protected override void LoadKey1()
//        {
//            m_key1.LoadValue(m_stream);
//        }

//        protected override void SaveKey2()
//        {
//            m_key2.SaveValue(m_stream);
//        }

//        protected override void LoadKey2()
//        {
//            m_key2.LoadValue(m_stream);
//        }

//        protected override void SaveKey3()
//        {
//            m_key3.SaveValue(m_stream);
//        }

//        protected override void LoadKey3()
//        {
//            m_key3.LoadValue(m_stream);
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
//            return m_key1.CompareToStream(m_stream);
//        }

//        public void AddData(TKey key, TValue value)
//        {
//            m_key1 = key;
//            m_value1 = value;
//            AddData();
//        }

//        public TValue GetData(TKey key)
//        {
//            m_key1 = key;
//            GetData();
//            return m_value1;
//        }

//    }
//}
