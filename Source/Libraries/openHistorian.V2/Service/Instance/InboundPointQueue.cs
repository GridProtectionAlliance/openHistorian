using openHistorian.V2.IO.Unmanaged;

namespace openHistorian.V2.Service.Instance
{

    class InboundPointQueue
    {
        //ToDO: make the queue be an in memory b+ tree.  This may speed up the inserting 
        //to the main database.

        MemoryStream m_memoryStream1;
        MemoryStream m_memoryStream2;

        BinaryStream m_processingQueue;
        BinaryStream m_activeQueue;
        const int SizeOfData = 32;

        public InboundPointQueue()
        {
            m_memoryStream1 = new MemoryStream();
            m_activeQueue = new BinaryStream(m_memoryStream1);
            m_memoryStream2 = new MemoryStream();
            m_activeQueue = new BinaryStream(m_memoryStream2);

        }

        public void WriteData(long key1, long key2, long value1, long value2)
        {
            lock (this)
            {
                m_activeQueue.Write(key1);
                m_activeQueue.Write(key2);
                m_activeQueue.Write(value1);
                m_activeQueue.Write(value2);
            }
        }

        public void GetPointBlock(out BinaryStream stream, out int pointCount)
        {
            lock (this)
            {
                stream = m_activeQueue;
                pointCount = (int)(m_activeQueue.Position / SizeOfData);

                m_activeQueue = m_processingQueue;
                m_activeQueue.Position = 0;
                m_processingQueue = m_activeQueue;
            }
        }
    }
}
