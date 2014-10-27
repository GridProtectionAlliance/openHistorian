using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GSF.SortedTreeStore;
using GSF.SortedTreeStore.Services.Configuration;
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
    public class ConcurrentReading
    {
        const int PointsToRead = 10000000;
        int ReaderNumber = 0;
        int ThreadNumber = 0;
        volatile bool StopReading;

        [Test]
        public void ScanAllPoints()
        {
            Stats.Clear();
            long points;

            StopReading = false;

            var settings = new HistorianServerDatabaseConfig("PPA", @"C:\Program Files\openHistorian\Archive\", true);
            using (HistorianServer server = new HistorianServer(settings))
            {
                Thread.Sleep(1000);

                for (int x = 1; x < 30; x++)
                {
                    StartScanner();
                    Thread.Sleep(1000);
                    if (x == 1)
                        Thread.Sleep(5000);
                    Interlocked.Exchange(ref Stats.PointsReturned, 0);
                    Thread.Sleep(1000);
                    long v = Interlocked.Read(ref Stats.PointsReturned);
                    Console.WriteLine("Clients: " + x.ToString() + " points " + v.ToString());
                }

                StopReading = true;
                Thread.Sleep(2000);
            }
            Thread.Sleep(2000);

        }


        [Test]
        public void SendAllPoints()
        {
            Stats.Clear();
            long points;

            StopReading = false;

            var settings = new HistorianServerDatabaseConfig("PPA", @"C:\Program Files\openHistorian\Archive\", true);
            using (HistorianServer server = new HistorianServer(settings))
            {
                Thread.Sleep(1000);

                for (int x = 1; x < 30; x++)
                {
                    StartReader();
                    Thread.Sleep(1000);
                    if (x == 1)
                        Thread.Sleep(5000);
                    Interlocked.Exchange(ref Stats.PointsReturned, 0);
                    Thread.Sleep(1000);
                    long v = Interlocked.Read(ref Stats.PointsReturned);
                    Console.WriteLine("Clients: " + x.ToString() + " points " + v.ToString());
                }

                StopReading = true;
                Thread.Sleep(2000);
            }
            Thread.Sleep(2000);

        }

        void StartScanner()
        {
            Thread th = new Thread(ScannerThread);
            th.IsBackground = true;
            th.Start();
        }


        void ScannerThread()
        {
            int threadId = Interlocked.Increment(ref ThreadNumber);

            try
            {

                //DateTime start = DateTime.FromBinary(Convert.ToDateTime("2/1/2014").Date.Ticks + Convert.ToDateTime("6:00:00PM").TimeOfDay.Ticks).ToUniversalTime();
                while (!StopReading)
                {

                    Stopwatch sw = new Stopwatch();
                    NetworkClientConfig clientConfig = new NetworkClientConfig();
                    clientConfig.NetworkPort = 12345;
                    clientConfig.ServerNameOrIp = "127.0.0.1";

                    using (var client = new HistorianClient(clientConfig))
                    using (var database = client.GetDatabase<HistorianKey, HistorianValue>(String.Empty))
                    {
                        HistorianKey key = new HistorianKey();
                        HistorianValue value = new HistorianValue();

                        sw.Start();
                        var scan = database.Read(0, ulong.MaxValue, new ulong[] { 65, 953, 5562 });
                        while (scan.Read(key, value))
                            ;
                        sw.Stop();
                    }

                    //Console.WriteLine("Thread: " + threadId.ToString() + " " + "Run Number: " + myId.ToString() + " " + (pointCount / sw.Elapsed.TotalSeconds / 1000000).ToString());
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Thread: " + threadId.ToString() + " Quit");
        }

        void StartReader()
        {
            Thread th = new Thread(ReaderThread);
            th.IsBackground = true;
            th.Start();
        }


        void ReaderThread()
        {
            int threadId = Interlocked.Increment(ref ThreadNumber);

            try
            {

                DateTime start = DateTime.FromBinary(Convert.ToDateTime("2/1/2014").Date.Ticks + Convert.ToDateTime("6:00:00PM").TimeOfDay.Ticks).ToUniversalTime();
                while (!StopReading)
                {

                    int myId = Interlocked.Increment(ref ReaderNumber);
                    Stopwatch sw = new Stopwatch();
                    int pointCount = 0;
                    NetworkClientConfig clientConfig = new NetworkClientConfig();
                    clientConfig.NetworkPort = 12345;
                    clientConfig.ServerNameOrIp = "127.0.0.1";

                    using (var client = new HistorianClient(clientConfig))
                    using (var database = client.GetDatabase<HistorianKey, HistorianValue>(String.Empty))
                    {
                        HistorianKey key = new HistorianKey();
                        HistorianValue value = new HistorianValue();

                        sw.Start();
                        var scan = database.Read((ulong)start.Ticks, ulong.MaxValue);//, new ulong[] { 65, 953, 5562 });
                        while (scan.Read(key, value) && pointCount < PointsToRead)
                            pointCount++;
                        sw.Stop();
                    }

                    //Console.WriteLine("Thread: " + threadId.ToString() + " " + "Run Number: " + myId.ToString() + " " + (pointCount / sw.Elapsed.TotalSeconds / 1000000).ToString());
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Thread: " + threadId.ToString() + " Quit");
        }


    }
}
