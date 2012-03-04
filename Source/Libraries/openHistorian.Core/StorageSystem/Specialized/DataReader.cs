using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core.StorageSystem.Specialized
{
    public class DataReader<TKey, TValue>
        where TKey : struct, IKeyType<TKey>
        where TValue : struct, IValueType<TValue>
    {

        ITreeLeafNodeMethods<TKey> m_leafMethods;
        TValue m_value;
        bool m_isValueValid;
        TKey m_key;
        bool m_isKeyValid;
        BinaryStream m_stream;

        public DataReader(ITreeLeafNodeMethods<TKey> leafMethods, BinaryStream stream)
        {
            m_value = default(TValue);
            m_stream = stream;
            m_leafMethods = leafMethods;
            m_isKeyValid = false;
            m_isValueValid = false;
        }
        public bool Next()
        {
            m_isValueValid = false;
            m_isKeyValid = m_leafMethods.GetNextKeyTableScan(out m_key);
            return m_isKeyValid;
        }
        public TKey GetKey()
        {
            return m_key;
        }
        public TValue GetValue()
        {
            if (!m_isKeyValid)
                throw new Exception("Key is no longer valid.  Either the end of the stream has been encoutered or the initial read was never performed");
            if (!m_isValueValid)
            {
                m_value.LoadValue(m_stream);
                m_isValueValid = true;
            }
            return m_value;
        }
    }
}
