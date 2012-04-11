using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using openHistorian.V2.Unmanaged;

namespace openHistorian.V2
{

    class IncommingQueue
    {
        //ToDO: make the queue be an in memory b+ tree.  This may speed up the inserting 
        //to the main database.

        MemoryStream m_memoryStream1;
        MemoryStream m_memoryStream2;

        BinaryStream m_processingQueue;
        BinaryStream m_activeQueue;
        const int SizeOfData = 20;

        public IncommingQueue()
        {
            m_memoryStream1 = new MemoryStream();
            m_activeQueue = new BinaryStream(m_memoryStream1);
            m_memoryStream2 = new MemoryStream();
            m_activeQueue = new BinaryStream(m_memoryStream2);

        }

        public void WriteData(IDataPoint dataPoint)
        {
            lock (this)
            {
                m_activeQueue.Write((long)dataPoint.Time);
                m_activeQueue.Write(dataPoint.HistorianID);
                m_activeQueue.Write(dataPoint.Flags);
                m_activeQueue.Write(dataPoint.Value);
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
