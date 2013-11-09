using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using GSF.IO.Unmanaged;
using GSF.SortedTreeStore.Net;
using openHistorian;
using GSF.SortedTreeStore.Storage;
using openHistorian.Collections;
using GSF.SortedTreeStore.Tree;
using GSF.SortedTreeStore.Tree.TreeNodes;
using openHistorian.Data;
using openHistorian.Data.Query;

namespace HistorianDemos
{
    class UserForumDemoCode
    {
        int portNumber = 12345;


        private void ReadData1_Click(object sender, EventArgs e)
        {
            portNumber++;
            HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            db.InMemoryArchive = true;
            db.ConnectionString = "port=" + portNumber;
            db.Paths = new[] { @"c:\ActualData\" };

            using (HistorianServer server = new HistorianServer(db))
            {
                HistorianClientOptions clientOptions = new HistorianClientOptions();
                clientOptions.NetworkPort = portNumber;
                clientOptions.ServerNameOrIp = "127.0.0.1";

                using (var client = new HistorianClient(clientOptions))
                {
                    var startTime = new DateTime(2013, 8, 12);
                    var stopTime = startTime.AddSeconds(1);

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    var sig = client.GetDefaultDatabase().GetFrames(startTime, stopTime);
                    sw.Stop();

                    var sb = new StringBuilder();
                    foreach (var f in sig)
                    {
                        foreach (var v in f.Value.Points)
                        {
                            sb.AppendLine(f.Key.ToString("MM/dd/yy H:mm:ss.fff") + "\t"
                                          + f.Key.ToString("MM/dd/yy H:mm:ss.fffffff") + "\t"
                                          + v.Key.ToString() + "\t"
                                          + v.Value.AsSingle.ToString());
                        }
                    }
                    //Clipboard.SetText(sb.ToString());
                    Console.Write(sw.Elapsed.TotalMilliseconds.ToString());
                }
            }
        }

        private void ReadData2_Click(object sender, EventArgs e)
        {
            portNumber++;
            HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            db.InMemoryArchive = true;
            db.ConnectionString = "port=" + portNumber;
            db.Paths = new[] { @"c:\ActualData\" };

            using (HistorianServer server = new HistorianServer(db))
            {
                HistorianClientOptions clientOptions = new HistorianClientOptions();
                clientOptions.NetworkPort = portNumber;
                clientOptions.ServerNameOrIp = "127.0.0.1";

                using (var client = new HistorianClient(clientOptions))
                {
                    var startTime = new DateTime(2013, 8, 12);
                    var stopTime = startTime.AddSeconds(1);

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    var sig = client.GetDefaultDatabase().GetFrames(startTime, stopTime, 3736, 4466);
                    sw.Stop();

                    var sb = new StringBuilder();
                    foreach (var f in sig)
                    {
                        foreach (var v in f.Value.Points)
                        {
                            sb.AppendLine(f.Key.ToString("MM/dd/yy H:mm:ss.fff") + "\t"
                                          + f.Key.ToString("MM/dd/yy H:mm:ss.fffffff") + "\t"
                                          + v.Key.ToString() + "\t"
                                          + v.Value.AsSingle.ToString());
                        }
                    }
                    //Clipboard.SetText(sb.ToString());
                    Console.Write(sw.Elapsed.TotalMilliseconds.ToString());
                }
            }
        }

        private void ReadData3_Click(object sender, EventArgs e)
        {
            portNumber++;
            HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            db.InMemoryArchive = true;
            db.ConnectionString = "port=" + portNumber;
            db.Paths = new[] { @"c:\ActualData\" };

            using (HistorianServer server = new HistorianServer(db))
            {
                HistorianClientOptions clientOptions = new HistorianClientOptions();
                clientOptions.NetworkPort = portNumber;
                clientOptions.ServerNameOrIp = "127.0.0.1";

                using (var client = new HistorianClient(clientOptions))
                {
                    var startTime = new DateTime(2013, 8, 12);
                    var stopTime = startTime.AddSeconds(1);

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    var sig = client.GetDefaultDatabase().GetFrames(startTime, stopTime, 3736, 4466).RoundToTolerance(1);
                    sw.Stop();

                    var sb = new StringBuilder();
                    foreach (var f in sig)
                    {
                        foreach (var v in f.Value.Points)
                        {
                            sb.AppendLine(f.Key.ToString("MM/dd/yy H:mm:ss.fff") + "\t"
                                          + f.Key.ToString("MM/dd/yy H:mm:ss.fffffff") + "\t"
                                          + v.Key.ToString() + "\t"
                                          + v.Value.AsSingle.ToString());
                        }
                    }
                    //Clipboard.SetText(sb.ToString());
                    Console.Write(sw.Elapsed.TotalMilliseconds.ToString());
                }
            }
        }

        private void ReadData4_Click(object sender, EventArgs e)
        {
            portNumber++;
            HistorianDatabaseInstance db = new HistorianDatabaseInstance();
            db.InMemoryArchive = true;
            db.ConnectionString = "port=" + portNumber;
            db.Paths = new[] { @"c:\ActualData\" };

            using (HistorianServer server = new HistorianServer(db))
            {
                HistorianClientOptions clientOptions = new HistorianClientOptions();
                clientOptions.NetworkPort = portNumber;
                clientOptions.ServerNameOrIp = "127.0.0.1";

                using (var client = new HistorianClient(clientOptions))
                {
                    var startTime = new DateTime(2013, 8, 12);
                    var stopTime = startTime.AddMinutes(50);
                    var parser = new PeriodicScanner(30);
                    var times = parser.GetParser(startTime, stopTime, 1000u);

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    var sig = client.GetDefaultDatabase().GetFrames(times, 3736, 4466).RoundToTolerance(1);
                    sw.Stop();

                    var sb = new StringBuilder();
                    foreach (var f in sig)
                    {
                        foreach (var v in f.Value.Points)
                        {
                            sb.AppendLine(f.Key.ToString("MM/dd/yy H:mm:ss.fff") + "\t"
                                          + f.Key.ToString("MM/dd/yy H:mm:ss.fffffff") + "\t"
                                          + v.Key.ToString() + "\t"
                                          + v.Value.AsSingle.ToString());
                        }
                    }
                    //Clipboard.SetText(sb.ToString());
                    Console.Write(sw.Elapsed.TotalMilliseconds.ToString());
                }
            }
        }

        private void ReadData5_Click(object sender, EventArgs e)
        {

            string oldFileName = @"C:\Archive\archive1.d";
            string newFileName = @"C:\Archive\archive1.d2";
            //Guid compressionMethod = CreateFixedSizeNode.TypeGuid;

            if (!File.Exists(oldFileName))
                throw new ArgumentException("Old file does not exist", "oldFileName");
            if (File.Exists(newFileName))
                File.Delete(newFileName);

            Stopwatch swPreSort = new Stopwatch();
            Stopwatch swMigrate = new Stopwatch();
            Stopwatch swReRead = new Stopwatch();


            int count2 = 0;

            int count = 0;

            using (var file1 = SortedTreeFile.CreateInMemory())
            using (var table1 = file1.OpenOrCreateTable<HistorianKey, HistorianValue>(SortedTree.FixedSizeNode))
            {
                using (var edit1 = table1.BeginEdit())
                {
                    var hist = new OldHistorianReader(oldFileName);

                    var key = new HistorianKey();
                    var value = new HistorianValue();
                    Func<OldHistorianReader.Points, bool> del = (x) =>
                    {
                        key.TimestampAsDate = x.Time;
                        key.PointID = (ulong)x.PointID;
                        value.Value3 = x.flags;
                        value.AsSingle = x.Value;
                        edit1.AddPoint(key, value);
                        count++;
                        return true;
                    };

                    swPreSort.Start();
                    hist.Read(del);

                    edit1.Commit();
                    swPreSort.Stop();
                }
                using (var read = table1.BeginRead())
                {
                    swMigrate.Start();
                    using (var file2 = SortedTreeFile.CreateFile(newFileName))
                    using (var table2 = file2.OpenOrCreateTable<HistorianKey, HistorianValue>(CreateHistorianCompressionTs.TypeGuid))
                    {
                        using (var edit2 = table2.BeginEdit())
                        {
                            var scan0 = read.GetTreeScanner();
                            scan0.SeekToStart();
                            while (scan0.Read())
                            {
                                edit2.AddPoint(scan0.CurrentKey, scan0.CurrentValue);
                            }
                            edit2.Commit();
                        }
                        swMigrate.Stop();
                        swReRead.Start();
                        using (var read3 = table2.BeginRead())
                        {
                            var scan0 = read3.GetTreeScanner();
                            scan0.SeekToStart();
                            while (scan0.Read())
                            {
                                count2++;
                            }
                        }
                        swReRead.Stop();

                    }

                    Console.Write(count.ToString() + "\t" + count2.ToString());
                    Console.Write((count / swPreSort.Elapsed.TotalSeconds / 1000000).ToString() + "\n" +
                     (count / swMigrate.Elapsed.TotalSeconds / 1000000).ToString() + "\n" +
                     (count / swReRead.Elapsed.TotalSeconds / 1000000).ToString());

                }
            }
        }
    }
}
