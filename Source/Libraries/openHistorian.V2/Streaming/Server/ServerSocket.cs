//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Net.Sockets;
//using System.Threading;
//using openHistorian.V2.Local;
//using openHistorian.V2.Server;

//namespace openHistorian.V2.Streaming.Server
//{
//    class ServerSocket
//    {
//        TcpListener m_listener;
//        Thread m_acceptClientThread;
//        IHistorian m_historian;
        
//        public ServerSocket(int port, string configFile)
//        {
//            m_historian = new HistorianServer(configFile);
//            m_listener = new TcpListener(new IPAddress(0), port);
//            m_acceptClientThread = new Thread(AcceptClient);
//            m_acceptClientThread.Start();
//        }

//        void AcceptClient()
//        {
//            m_listener.Start();
//            TcpClient client = m_listener.AcceptTcpClient();
//            ITransportStreaming stream=null;
//            ProcessClientStream cs = new ProcessClientStream(stream,(HistorianEngine)m_historian);

//        }


//    }
//}
