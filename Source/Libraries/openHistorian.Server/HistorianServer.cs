using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using openHistorian.Communications;
using openHistorian.Engine;

namespace openHistorian
{
    public class HistorianServerOptions
    {
        public bool IsNetworkHosted = false;
        public List<string> Paths = new List<string>();
        public bool IsReadOnly = true;
        public int NetworkPort = 38402;
    }

    public class HistorianServer
    {
        SocketHistorian m_socket;
        HistorianDatabaseCollection m_databases;

        public HistorianServer(HistorianServerOptions options)
        {
            m_databases = new HistorianDatabaseCollection();
            if (options.IsReadOnly)
            {
                var database = new ArchiveDatabaseEngine(null, options.Paths.ToArray());
                m_databases.Add("default", database);
            }
            else
            {
                var database = new ArchiveDatabaseEngine(WriterOptions.IsFileBased(), options.Paths.ToArray());
                m_databases.Add("default", database);
            }

            if (options.IsNetworkHosted)
            {
                m_socket=new SocketHistorian(options.NetworkPort,m_databases);
            }
        }
    }
}
