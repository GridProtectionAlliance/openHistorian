//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;

//namespace GSF.IO
//{
//    public interface IWritePrepare
//    {

//    }
//    class AsyncWriter
//    {
//        object m_syncObject;
//        FileStream m_stream;
//        int m_maxQueueDepth;

//        int m_counter;
//        bool m_isDone;
//        ManualResetEvent WaitComplete;

//        public AsyncWriter(FileStream stream, object syncObject, int maxQueueDepth)
//        {
//            m_stream = stream;
//            m_syncObject = syncObject;
//            m_maxQueueDepth = maxQueueDepth;
//        }

//        public void WriteToDisk(long position, byte[] buffer, int length)
//        {
//            IAsyncResult results;
//            lock (m_syncObject)
//            {
//                m_stream.Position = position;
//                results = m_stream.BeginWrite(buffer, 0, length, BeginWriteCallback, null);
//            }
//            m_stream.EndWrite(results);
//        }

//        void BeginWriteCallback(IAsyncResult ar)
//        {
//            m_stream.EndWrite(ar);
//        }

//        public void ItemQueued()
//        {

//        }
//        public void ItemDequeued()
//        {

//        }
//    }
//}

