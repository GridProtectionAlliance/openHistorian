using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using openHistorian.Communications;
using System.Net;

namespace openHistorian
{
    public class HistorianClientOptions
    {
        public bool IsReadOnly = true;
        public int NetworkPort = 38402;
        public string ServerNameOrIp = "localhost";
        public string DefaultDatabase = "default";
    }

    public class HistorianClient : IHistorianDatabase
    {

        RemoteHistorian m_historian;
        string m_defaultDatabase;
        bool m_connectedToDefault;
        IHistorianDatabase m_currentConnection;

        public HistorianClient(HistorianClientOptions options)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(options.ServerNameOrIp, out ip))
            {
                ip = Dns.GetHostAddresses(options.ServerNameOrIp)[0];
            }
            m_historian = new RemoteHistorian(new IPEndPoint(ip, options.NetworkPort));
            m_currentConnection = m_historian.ConnectToDatabase(options.DefaultDatabase);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens a stream connection that can be used to read 
        /// and write data to the current historian database.
        /// </summary>
        /// <returns></returns>
        public IHistorianDataReader OpenDataReader()
        {
            throw new NotImplementedException();
        }

        public void Write(IPointStream points)
        {
            throw new NotImplementedException();
        }

        public void Write(ulong key1, ulong key2, ulong value1, ulong value2)
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void CommitToDisk()
        {
            throw new NotImplementedException();
        }
    }
}
