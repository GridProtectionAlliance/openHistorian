using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GSF.SortedTreeStore.Services.Net;
using GSF.SortedTreeStore.Services.Reader;
using GSF.SortedTreeStore.Net;
using openHistorian.Collections;
using openHistorian.Data;

using NUnit.Framework;
using openHistorian.Data.Query;

namespace openHistorian.PerformanceTests
{
    [TestFixture]
    public class ReadPoints
    {
        [Test]
        public void ReadFrames()
        {
            throw new NotImplementedException();

            //Stopwatch sw = new Stopwatch();
            //int pointCount = 0;
            //HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            //db.InMemoryArchive = true;
            //db.ConnectionString = "port=12345";
            //db.Paths = new[] { @"C:\Program Files\openHistorian\Archive\" };

            //using (HistorianServer server = new HistorianServer(db))
            //{
            //    SortedTreeClientOptions clientOptions = new SortedTreeClientOptions();
            //    clientOptions.NetworkPort = 12345;
            //    clientOptions.ServerNameOrIp = "127.0.0.1";

            //    using (var client = new HistorianClient(clientOptions))
            //    {
            //        var database = server.GetDefaultDatabase();

            //        using (var frameReader = database.GetPointStream(DateTime.MinValue, DateTime.MaxValue).GetFrameReader())
            //        {
            //            while (frameReader.Read())
            //                ;
            //        }


            //        sw.Start();
            //        using (var frameReader = database.GetPointStream(DateTime.MinValue, DateTime.MaxValue).GetFrameReader())
            //        {
            //            while (frameReader.Read())
            //                ;
            //        }
            //        sw.Stop();
            //    }
            //}
            //Console.WriteLine(pointCount);
            //Console.WriteLine(sw.Elapsed.TotalSeconds.ToString());

        }

        [Test]
        public static void ReadAllPoints()
        {
            Stopwatch sw = new Stopwatch();
            int pointCount = 0;

            using (HistorianServer server = new HistorianServer(@"C:\Program Files\openHistorian\Archive\"))
            {
                NetworkClientConfig clientConfig = new NetworkClientConfig();
                clientConfig.NetworkPort = 12345;
                clientConfig.ServerNameOrIp = "127.0.0.1";

                DateTime start = DateTime.FromBinary(Convert.ToDateTime("2/1/2014").Date.Ticks + Convert.ToDateTime("6:00:00PM").TimeOfDay.Ticks).ToUniversalTime();

                using (var client = new HistorianClient(clientConfig))
                using (var database = client.GetDatabase<HistorianKey, HistorianValue>(String.Empty))
                {
                    HistorianKey key = new HistorianKey();
                    HistorianValue value = new HistorianValue();

                    sw.Start();
                    var scan = database.Read((ulong)start.Ticks, ulong.MaxValue);
                    while (scan.Read(key, value) && pointCount < 10000000)
                        pointCount++;
                    sw.Stop();

                    //sw.Start();
                    //using (var frameReader = database.GetPointStream(DateTime.MinValue, DateTime.MaxValue))
                    //{
                    //    while (frameReader.Read())
                    //        ;
                    //}
                    //sw.Stop();
                }
            }
            Console.WriteLine(pointCount);
            Console.WriteLine(sw.Elapsed.TotalSeconds.ToString());
            Console.WriteLine((pointCount / sw.Elapsed.TotalSeconds / 1000000).ToString());

        }

        [Test]
        public static void ReadAllPointsServer()
        {
            throw new NotImplementedException();

            //Stopwatch sw = new Stopwatch();
            //int pointCount = 0;
            //HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            //db.InMemoryArchive = true;
            //db.ConnectionString = "port=12345";
            //db.Paths = new[] { @"C:\Program Files\openHistorian\Archive\" };

            //using (HistorianServer server = new HistorianServer(db))
            //{
            //    SortedTreeClientOptions clientOptions = new SortedTreeClientOptions();
            //    clientOptions.NetworkPort = 12345;
            //    clientOptions.ServerNameOrIp = "127.0.0.1";

            //    //using (var client = new HistorianClient(clientOptions))
            //    //{
            //    var database = server.GetDefaultDatabase();

            //    HistorianKey key = new HistorianKey();
            //    HistorianValue value = new HistorianValue();

            //    var scan = database.Read(0, ulong.MaxValue);
            //    while (scan.Read(key, value))// && pointCount < 1000000)
            //        ;

            //    sw.Start();
            //    scan = database.Read(0, ulong.MaxValue);
            //    while (scan.Read(key, value))// && pointCount < 1000000)
            //        pointCount++;
            //    //using (var frameReader = database.GetPointStream(DateTime.MinValue, DateTime.MaxValue))
            //    //{
            //    //    while (frameReader.Read())// && pointCount < 1000000)
            //    //        pointCount++;
            //    //}
            //    sw.Stop();

            //    //sw.Start();
            //    //using (var frameReader = database.GetPointStream(DateTime.MinValue, DateTime.MaxValue))
            //    //{
            //    //    while (frameReader.Read())
            //    //        ;
            //    //}
            //    //sw.Stop();
            //    //}
            //}
            //Console.WriteLine(pointCount);
            //Console.WriteLine(sw.Elapsed.TotalSeconds.ToString());
            //Console.WriteLine((pointCount / sw.Elapsed.TotalSeconds / 1000000).ToString());

        }

        [Test]
        public void TestReadPoints()
        {
            Stopwatch sw = new Stopwatch();
            int pointCount = 0;

            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();

            using (HistorianServer server = new HistorianServer(@"C:\Program Files\openHistorian\Archive\"))
            {
                NetworkClientConfig clientConfig = new NetworkClientConfig();
                clientConfig.NetworkPort = 12345;
                clientConfig.ServerNameOrIp = "127.0.0.1";

                using (var client = new HistorianClient(clientConfig))
                using (var database = client.GetDatabase<HistorianKey, HistorianValue>(String.Empty))
                {

                    var stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, new ulong[] { 1 });
                    while (stream.Read(key, value))
                        ;

                    sw.Start();
                    stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, new ulong[] { 65, 953, 5562 });
                    while (stream.Read(key, value))
                        pointCount++;

                    sw.Stop();
                }
            }
            Console.WriteLine(pointCount);
            Console.WriteLine(sw.Elapsed.TotalSeconds.ToString());
        }

        [Test]
        public static void TestReadFilteredPoints()
        {
            throw new NotImplementedException();

            //Stopwatch sw = new Stopwatch();
            //int pointCount = 0;
            //HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            //db.InMemoryArchive = true;
            //db.ConnectionString = "port=12345";
            //db.Paths = new[] { @"C:\Program Files\openHistorian\Archive\" };

            //using (HistorianServer server = new HistorianServer(db))
            //{
            //    SortedTreeClientOptions clientOptions = new SortedTreeClientOptions();
            //    clientOptions.NetworkPort = 12345;
            //    clientOptions.ServerNameOrIp = "127.0.0.1";

            //    //using (var client = new HistorianClient(clientOptions))
            //    //{
            //    var database = server.GetDefaultDatabase();
            //    var stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, new ulong[] { 1 });
            //    while (stream.Read())
            //        ;

            //    sw.Start();
            //    stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, new ulong[] { 65, 953, 5562 });
            //    while (stream.Read())
            //        pointCount++;

            //    sw.Stop();
            //    //}
            //}
            //Console.WriteLine(pointCount);
            //Console.WriteLine(sw.Elapsed.TotalSeconds.ToString());
            //Console.WriteLine((140107816 / sw.Elapsed.TotalSeconds / 1000000).ToString());
        }

        [Test]
        public void TestReadFilteredPointsAll()
        {
            throw new NotImplementedException();

            //HistorianKey key = new HistorianKey();
            //HistorianValue value = new HistorianValue();
            //Stopwatch sw = new Stopwatch();
            //int pointCount = 0;
            //HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            //db.InMemoryArchive = true;
            //db.ConnectionString = "port=12345";
            //db.Paths = new[] { @"C:\Program Files\openHistorian\Archive\" };

            //var lst = new List<ulong>();
            //for (uint x = 0; x < 6000; x++)
            //{
            //    lst.Add(x);
            //}



            //using (HistorianServer server = new HistorianServer(db))
            //{
            //    SortedTreeClientOptions clientOptions = new SortedTreeClientOptions();
            //    clientOptions.NetworkPort = 12345;
            //    clientOptions.ServerNameOrIp = "127.0.0.1";

            //    //using (var client = new HistorianClient(clientOptions))
            //    //{
            //    var database = server.GetDefaultDatabase();
            //    var stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, new ulong[] { 1 });
            //    while (stream.Read(key, value))
            //        ;

            //    sw.Start();
            //    stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, lst);
            //    while (stream.Read(key, value))
            //        pointCount++;

            //    sw.Stop();
            //    //}
            //}
            //Console.WriteLine(pointCount);
            //Console.WriteLine(sw.Elapsed.TotalSeconds.ToString());
            //Console.WriteLine((140107816 / sw.Elapsed.TotalSeconds / 1000000).ToString());
        }


        public static void TestReadPoints2()
        {
            int pointCount = 0;
            HistorianKey key = new HistorianKey();
            HistorianValue value = new HistorianValue();
            using (HistorianServer server = new HistorianServer(@"C:\Program Files\openHistorian\Archive\"))
            {
                NetworkClientConfig clientConfig = new NetworkClientConfig();
                clientConfig.NetworkPort = 12345;
                clientConfig.ServerNameOrIp = "127.0.0.1";
                Stopwatch sw = new Stopwatch();
                sw.Start();
                using (var client = new HistorianClient(clientConfig))
                using (var database = client.GetDatabase<HistorianKey, HistorianValue>(String.Empty))
                {

                    var stream = database.Read(0, (ulong)DateTime.MaxValue.Ticks, new ulong[] { 65, 953, 5562 });
                    while (stream.Read(key, value))
                        pointCount++;

                }
                sw.Stop();
                //MessageBox.Show(sw.Elapsed.TotalSeconds.ToString());
            }
        }
    }
}
