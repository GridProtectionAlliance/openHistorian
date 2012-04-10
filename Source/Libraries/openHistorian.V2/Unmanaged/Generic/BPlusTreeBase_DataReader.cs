using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.V2.Unmanaged.Generic
{

    public abstract partial class BPlusTreeBase<TKey, TValue>
    {

        public class DataReader
        {
            BPlusTreeBase<TKey, TValue> m_leafMethods;
            TValue m_value;
            bool m_isValueValid;
            TKey m_key;
            bool m_isKeyValid;

            public DataReader(BPlusTreeBase<TKey, TValue> leafMethods)
            {
                m_value = default(TValue);
                m_leafMethods = leafMethods;
                m_isKeyValid = false;
                m_isValueValid = false;
            }
            public bool Next()
            {
                m_isValueValid = false;
                m_isKeyValid = m_leafMethods.LeafNodeGetNextKeyTableScan(out m_key);
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
                    m_value= m_leafMethods.LoadValue(m_leafMethods.m_leafNodeStream);
                    m_isValueValid = true;
                }
                return m_value;
            }
            public void Close()
            {
                m_leafMethods.LeafNodeCloseTableScan();
            }
        }
    }

}
