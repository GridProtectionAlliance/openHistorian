using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using openHistorian;
using openHistorian.Communications;

namespace openVisN
{
 
    public class HistorianQuery
    {
        RemoteHistorian m_historian;
        public HistorianQuery(string server, int port)
        {
            var ip = Dns.GetHostAddresses(server)[0];
            m_historian = new RemoteHistorian(new IPEndPoint(ip, port));
        }

        public QueryResults GetQueryResult(DateTime startTime, DateTime endTime, int zoomLevel, List<TerminalPoints> terminals)
        {
            ulong startKey = (ulong)startTime.Ticks;
            ulong endKey = (ulong)endTime.Ticks;
            List<ulong> points = new List<ulong>(terminals.Count * 10);

            //SortedList<PointID, <List<Time,Value>>>
            var results = new QueryResults();

            foreach (var terminal in terminals)
            {
                results.AddPointIfNotExists(terminal.CurrentAngle);
                results.AddPointIfNotExists(terminal.CurrentMagnitude);
                results.AddPointIfNotExists(terminal.VoltageAngle);
                results.AddPointIfNotExists(terminal.VoltageMagnitude);
                results.AddPointIfNotExists(terminal.Dfdt);
                results.AddPointIfNotExists(terminal.Frequency);
                results.AddPointIfNotExists(terminal.Status);
            }

            points.AddRange(results.GetAllPoints());

            using (var db = m_historian.ConnectToDatabase("Full Resolution Synchrophasor"))
            using (var reader = db.OpenDataReader())
            {
                var stream = reader.Read(startKey, endKey, points);
                ulong time, point, quality, value;
                while (stream.Read(out time, out point, out quality, out value))
                {
                    results.AddPoint(time, point, value);
                }
            }
            return results;
        }

    }

}
