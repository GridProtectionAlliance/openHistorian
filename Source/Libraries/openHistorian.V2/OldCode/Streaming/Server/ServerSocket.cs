//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Net.Sockets;
//using System.Threading;

//namespace openHistorian.Streaming.Server
//{
//    class ServerSocket
//    {
//        TcpListener m_listener;
//        Thread m_acceptClientThread;

//        public ServerSocket(int port)
//        {
//            m_listener = new TcpListener(new IPAddress(0), port);
//            m_acceptClientThread = new Thread(AcceptClient);
//            m_acceptClientThread.Start();
//        }

//        void AcceptClient()
//        {
//            m_listener.Start();
//            TcpClient client = m_listener.AcceptTcpClient();
            
//        }


//    }
//}
