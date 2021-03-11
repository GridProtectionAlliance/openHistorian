using System.Threading;
using GSF.Diagnostics;
using GSF.IO;
using GSF.Security;
using GSF.Snap.Storage;
using GSF.Snap.Services.Reader;
using GSF.Snap.Tree;
using NUnit.Framework;
using openHistorian.Net;
using openHistorian.Snap;

namespace GSF.Snap.Services.Net
{
    [TestFixture]
    public class StreamingClientServerTest
    {
        [Test]
        public void Test1()
        {
            Logger.Console.Verbose = VerboseLevel.All;

            NetworkStreamSimulator netStream = new NetworkStreamSimulator();

            HistorianServerDatabaseConfig dbcfg = new HistorianServerDatabaseConfig("DB", @"C:\Archive", true);
            HistorianServer server = new HistorianServer(dbcfg);
            SecureStreamServer<SocketUserPermissions> auth = new SecureStreamServer<SocketUserPermissions>();
            auth.SetDefaultUser(true, new SocketUserPermissions()
            {
                CanRead = true,
                CanWrite = true,
                IsAdmin = true
            });

            SnapStreamingServer netServer = new SnapStreamingServer(auth, netStream.ServerStream, server.Host);

            ThreadPool.QueueUserWorkItem(ProcessClient, netServer);

            SnapStreamingClient client = new SnapStreamingClient(netStream.ClientStream, new SecureStreamClientDefault(), true);

            ClientDatabaseBase db = client.GetDatabase("DB");

            client.Dispose();
            server.Dispose();
        }

        [Test]
        public void TestFile()
        {
            //		FilePath	"C:\\Temp\\Historian\\635287587300536177-Stage1-d559e63e-d938-46a9-8d57-268f7c8ba194.d2"	string
            //635329017197429979-Stage1-38887e11-4097-4937-b269-ce4037157691.d2
            //using (var file = SortedTreeFile.OpenFile(@"C:\Temp\Historian\635287587300536177-Stage1-d559e63e-d938-46a9-8d57-268f7c8ba194.d2", true))
            using (SortedTreeFile file = SortedTreeFile.OpenFile(@"C:\Archive\635329017197429979-Stage1-38887e11-4097-4937-b269-ce4037157691.d2", true))
            //using (var file = SortedTreeFile.OpenFile(@"C:\Temp\Historian\635255664136496199-Stage2-6e758046-b2af-40ff-ae4e-85cd0c0e4501.d2", true))
            using (SortedTreeTable<HistorianKey, HistorianValue> table = file.OpenTable<HistorianKey, HistorianValue>())
            using (SortedTreeTableReadSnapshot<HistorianKey, HistorianValue> reader = table.BeginRead())
            {
                SortedTreeScannerBase<HistorianKey, HistorianValue> scanner = reader.GetTreeScanner();
                scanner.SeekToStart();
                scanner.TestSequential().Count();

                HistorianKey key = new HistorianKey();
                HistorianValue value = new HistorianValue();

                int x = 0;
                scanner.Peek(key, value);

                while (scanner.Read(key, value) && x < 10000)
                {
                    System.Console.WriteLine(key.PointID);
                    x++;

                    scanner.Peek(key, value);
                }

                scanner.Count();
            }
        }

        [Test]
        public void Test2()
        {
            Logger.Console.Verbose = VerboseLevel.All;

            NetworkStreamSimulator netStream = new NetworkStreamSimulator();

            HistorianServerDatabaseConfig dbcfg = new HistorianServerDatabaseConfig("DB", @"C:\Archive", true);
            HistorianServer server = new HistorianServer(dbcfg);
            SecureStreamServer<SocketUserPermissions> auth = new SecureStreamServer<SocketUserPermissions>();
            auth.SetDefaultUser(true, new SocketUserPermissions()
            {
                CanRead = true,
                CanWrite = true,
                IsAdmin = true
            });

            SnapStreamingServer netServer = new SnapStreamingServer(auth, netStream.ServerStream, server.Host);

            ThreadPool.QueueUserWorkItem(ProcessClient, netServer);

            SnapStreamingClient client = new SnapStreamingClient(netStream.ClientStream, new SecureStreamClientDefault(), true);

            ClientDatabaseBase<HistorianKey, HistorianValue> db = client.GetDatabase<HistorianKey, HistorianValue>("DB");
            long len = db.Read().Count();
            System.Console.WriteLine(len);

            client.Dispose();
            server.Dispose();
        }


        [Test]
        public void TestWriteServer()
        {
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            Logger.Console.Verbose = VerboseLevel.All;
            Logger.FileWriter.SetPath(@"C:\Temp\", VerboseLevel.All);

            NetworkStreamSimulator netStream = new NetworkStreamSimulator();

            HistorianServerDatabaseConfig dbcfg = new HistorianServerDatabaseConfig("DB", @"C:\Temp\Scada", true);
            HistorianServer server = new HistorianServer(dbcfg);
            SecureStreamServer<SocketUserPermissions> auth = new SecureStreamServer<SocketUserPermissions>();
            auth.SetDefaultUser(true, new SocketUserPermissions()
            {
                CanRead = true,
                CanWrite = true,
                IsAdmin = true
            });

            SnapStreamingServer netServer = new SnapStreamingServer(auth, netStream.ServerStream, server.Host);

            ThreadPool.QueueUserWorkItem(ProcessClient, netServer);

            SnapStreamingClient client = new SnapStreamingClient(netStream.ClientStream, new SecureStreamClientDefault(), false);

            ClientDatabaseBase<HistorianKey, HistorianValue> db = client.GetDatabase<HistorianKey, HistorianValue>("DB");
            for (uint x = 0; x < 1000; x++)
            {
                key.Timestamp = x;
                db.Write(key, value);
                break;
            }

            client.Dispose();
            server.Dispose();
        }

        private void ProcessClient(object netServer)
        {
            ((SnapStreamingServer)netServer).ProcessClient();
        }

    }
}
