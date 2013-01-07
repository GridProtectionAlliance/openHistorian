using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace openHistorian.Collections
{
    public interface ILoadable<T>
       where T : struct
    {
        int SizeOf { get; }
        int MaxSerializedSize { get; }
        int Save(byte[] data, int position, int length);
        void Load(byte[] data, int position, int length);
    }

    public class IsolatedQueueFileBacked<T>
        where T : struct, ILoadable<T>
    {
        IsolatedQueue<T> m_inboundQueue;
        IsolatedQueue<T> m_outboundQueue;
        bool m_isFileMode;
        string m_file;
        int m_maxInMemorySize;
        int m_maxCount;
        public IsolatedQueueFileBacked(string file, int maxInMemorySize)
        {
            T value = default(T);
            m_maxCount = maxInMemorySize / value.SizeOf;
            m_maxInMemorySize = maxInMemorySize;
            m_isFileMode = false;
            m_file = file;
        }

        public void Enqueue(T item)
        {
            if (m_inboundQueue.EstimateCount > m_maxInMemorySize)
            {
                
            }
            m_inboundQueue.Enqueue(item);
        }

        public bool TryDequeue(out T item)
        {
            if (!m_outboundQueue.TryDequeue(out item))
            {

            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Since the math for the count is pretty complex, this only gives an idea of the
        /// size of the structure.  In other words, a value of zero does not mean this list is empty.
        /// </summary>
        public long EstimateCount
        {
            get
            {
                throw new NotImplementedException();
                //return m_blocks.Count * (long)m_unitCount;
            }
        }

    }
}
