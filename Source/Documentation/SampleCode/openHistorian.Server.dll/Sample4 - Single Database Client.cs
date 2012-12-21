using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using openHistorian;
using openHistorian.Engine;

namespace SampleCode.openHistorian.Server.dll
{
    [TestFixture]
    public class Sample4
    {
        [Test]
        public void CreateAllDatabases()
        {
            Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);
            Array.ForEach(Directory.GetFiles(@"c:\temp\Synchrophasor\", "*.d2", SearchOption.AllDirectories), File.Delete);

            var serverOptions = new HistorianServerDatabaseCollectionOptions();
            serverOptions.IsNetworkHosted = false;

            var db = new HistorianServerDatabaseSettings();
            db.DatabaseName = "Scada";
            db.IsReadOnly = false;
            db.Paths.Add(@"c:\temp\Scada\");

            serverOptions.Databases.Add(db);

            db = new HistorianServerDatabaseSettings();
            db.DatabaseName = "Synchrophasor";
            db.IsReadOnly = false;
            db.Paths.Add(@"c:\temp\Synchrophasor\");

            serverOptions.Databases.Add(db);

            using (var server = new HistorianServer(serverOptions))
            {
                var dbCollection = server.GetDatabaseCollection();
                var database = dbCollection.ConnectToDatabase("Scada");

                for (ulong x = 0; x < 10000; x++)
                    database.Write(x, 0, 0, 0);
                database.HardCommit();
                database.Disconnect();

                database = dbCollection.ConnectToDatabase("Synchrophasor");
                for (ulong x = 0; x < 10000; x++)
                    database.Write(x, 0, 0, 0);
                database.HardCommit();
                database.Disconnect();
            }
        }

        [Test]
        public void TestReadData()
        {
            var serverOptions = new HistorianServerDatabaseCollectionOptions();
            serverOptions.IsNetworkHosted = true;
            serverOptions.NetworkPort = 12345;

            var db = new HistorianServerDatabaseSettings();
            db.DatabaseName = "Scada";
            db.IsReadOnly = true;
            db.Paths.Add(@"c:\temp\Scada\");

            serverOptions.Databases.Add(db);

            db = new HistorianServerDatabaseSettings();
            db.DatabaseName = "Synchrophasor";
            db.IsReadOnly = true;
            db.Paths.Add(@"c:\temp\Synchrophasor\");

            serverOptions.Databases.Add(db);

            using (var server = new HistorianServer(serverOptions))
            {
                var clientOptions = new HistorianClientOptions();
                clientOptions.IsReadOnly = true;
                clientOptions.NetworkPort = 12345;
                clientOptions.ServerNameOrIp = "127.0.0.1";

                using (var client = new HistorianClient(clientOptions))
                {
                    var dbCollection = client.GetDatabaseCollection();
                    var database = dbCollection.ConnectToDatabase("Scada");
                    using (var reader = database.OpenDataReader())
                    {
                        var stream = reader.Read(0, 100);
                        stream.Cancel();
                    }
                    database.Disconnect();

                    database = dbCollection.ConnectToDatabase("Synchrophasor");
                    using (var reader = database.OpenDataReader())
                    {
                        var stream = reader.Read(0, 100);
                        stream.Cancel();
                    }
                    database.Disconnect();
                }

            }
        }
    }
}
