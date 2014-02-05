using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GSF.SortedTreeStore.Engine.Reader;
using GSF.SortedTreeStore.Net;
using openHistorian.Data;

using NUnit.Framework;

namespace openHistorian.PerformanceTests
{
    [TestFixture]
    public class ReadPoints
    {
        [Test]
        public void TestReadPoints()
        {
            Stopwatch sw = new Stopwatch();
            int pointCount = 0;
            HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            db.InMemoryArchive = true;
            db.ConnectionString = "port=12345";
            db.Paths = new[] { @"C:\Program Files\openHistorian\Archive\" };

            using (HistorianServer server = new HistorianServer(db))
            {
                HistorianClientOptions clientOptions = new HistorianClientOptions();
                clientOptions.NetworkPort = 12345;
                clientOptions.ServerNameOrIp = "127.0.0.1";

                using (var client = new HistorianClient(clientOptions))
                {
                    var database = client.GetDefaultDatabase();
                    using (var reader = database.OpenDataReader())
                    {
                        var stream = reader.Read(0, (ulong)DateTime.MaxValue.Ticks, new ulong[] { 1 });
                        while (stream.Read())
                            ;
                    }

                    sw.Start();
                    using (var reader = database.OpenDataReader())
                    {
                        var stream = reader.Read(0, (ulong)DateTime.MaxValue.Ticks, new ulong[] { 65, 953, 5562 });
                        while (stream.Read())
                            pointCount++;

                    }
                    sw.Stop();
                    database.Disconnect();
                }
            }
            Console.WriteLine(pointCount);
            Console.WriteLine(sw.Elapsed.TotalSeconds.ToString());
        }


        public static void TestReadPoints2()
        {
            int pointCount = 0;
            HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            db.InMemoryArchive = true;
            db.ConnectionString = "port=12345";
            db.Paths = new[] { @"C:\Program Files\openHistorian\Archive\" };

            using (HistorianServer server = new HistorianServer(db))
            {
                HistorianClientOptions clientOptions = new HistorianClientOptions();
                clientOptions.NetworkPort = 12345;
                clientOptions.ServerNameOrIp = "127.0.0.1";
                Stopwatch sw = new Stopwatch();
                sw.Start();
                using (var client = new HistorianClient(clientOptions))
                {
                    var database = client.GetDefaultDatabase();

                    using (var reader = database.OpenDataReader())
                    {
                        var stream = reader.Read(0, (ulong)DateTime.MaxValue.Ticks, new ulong[] { 65, 953, 5562 });
                        while (stream.Read())
                            pointCount++;

                    }
                    database.Disconnect();
                }
                sw.Stop();
                //MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
            }
        }
    }
}
