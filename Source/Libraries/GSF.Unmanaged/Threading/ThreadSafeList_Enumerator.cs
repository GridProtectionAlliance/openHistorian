using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSF.Threading
{
    public partial class ThreadSafeList<T>
    {
        class Enumerator : IEnumerator<T>
        {
            T m_nextItem;
            bool m_disposed;
            bool m_nextItemExists;
            Iterator m_iterator;
            public Enumerator(Iterator iterator)
            {
                m_iterator = iterator;
                m_nextItemExists = false;
            }

            public void Dispose()
            {
                if (!m_disposed)
                {
                    if (m_nextItemExists)
                        m_iterator.UnsafeUnregisterItem();

                    m_disposed = true;
                    m_nextItemExists = false;
                    m_nextItem = default(T);
                    m_iterator = null;
                }
            }

            public bool MoveNext()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (m_nextItemExists)
                    m_iterator.UnsafeUnregisterItem();

                m_nextItemExists = m_iterator.UnsafeTryGetNextItem(out m_nextItem);
                return m_nextItemExists;
            }

            public void Reset()
            {
                if (m_disposed)
                    throw new ObjectDisposedException(GetType().FullName);
                if (m_nextItemExists)
                    m_iterator.UnsafeUnregisterItem();

                m_nextItem = default(T);
                m_nextItemExists = false;
                m_iterator.Reset();
            }

            public T Current
            {
                get
                {
                    if (!m_nextItemExists)
                        throw new InvalidOperationException("Past the end of the array, or never called MoveNext()");
                    return m_nextItem;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }
        }
    }
}
