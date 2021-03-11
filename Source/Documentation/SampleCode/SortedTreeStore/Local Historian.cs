using System;
using System.Threading;
using GSF;
using GSF.Diagnostics;
using GSF.Snap;
using GSF.Snap.Services;
using NUnit.Framework;
using System.Linq;
using openHistorian.Net;
using openHistorian.Snap;

namespace SampleCode.SortedTreeStore
{
    [TestFixture]
    public class Local_Historian
    {
        public SnapServer CreateServer()
        {
            HistorianServerDatabaseConfig settings = new HistorianServerDatabaseConfig("PPA", @"C:\Temp\Synchrophasor", true);
            SnapServer server = new SnapServer(settings);
            return server;
        }

        [Test]
        public void CreateHistorian()
        {
            Logger.Console.Verbose = VerboseLevel.All;

            using (SnapServer server = CreateServer())
            {

            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(1000);
        }

        [Test]
        public void AttachFile()
        {
            Logger.Console.Verbose = VerboseLevel.All;

            using (SnapServer server = CreateServer())
            {
                using (SnapClient client = SnapClient.Connect(server))
                {
                    client.GetDatabase("PPA").AttachFilesOrPaths(new string[] { @"C:\Temp\Synchrophasor\Dir\File2.d2" });
                }
            }

        }

        [Test]
        public void ReadData()
        {
            Logger.Console.Verbose = VerboseLevel.All;

            using (SnapServer server = CreateServer())
            {
                using (SnapClient client = SnapClient.Connect(server))
                using (ClientDatabaseBase<HistorianKey, HistorianValue> db = client.GetDatabase<HistorianKey, HistorianValue>("PPA"))
                using (TreeStream<HistorianKey, HistorianValue> stream = db.Read(null, null, null))
                {
                    Console.WriteLine(stream.Count());
                }
            }
        }

        [Test]
        public void WriteData()
        {
            Logger.Console.Verbose = VerboseLevel.All;

            using (SnapServer server = CreateServer())
            {
                using (SnapClient client = SnapClient.Connect(server))
                using (ClientDatabaseBase<HistorianKey, HistorianValue> db = client.GetDatabase<HistorianKey, HistorianValue>("PPA"))
                {
                    HistorianKey key = new HistorianKey();
                    HistorianValue value = new HistorianValue();
                    key.TimestampAsDate = DateTime.Now;
                    key.PointID = LittleEndian.ToUInt64(Guid.NewGuid().ToByteArray(), 0);
                    db.Write(key, value);
                }
            }
        }

        [Test]
        public void GetAllFiles()
        {
            Logger.Console.Verbose = VerboseLevel.All;

            using (SnapServer server = CreateServer())
            {
                using (SnapClient client = SnapClient.Connect(server))
                using (ClientDatabaseBase db = client.GetDatabase("PPA"))
                {
                    foreach (ArchiveDetails f in db.GetAllAttachedFiles())
                    {
                        Console.WriteLine("{0}MB {1} TO {2}; ID:{3} Name: {4}",
                            (f.FileSize / 1024d / 1024d).ToString("0.0"),
                            f.FirstKey, f.LastKey, f.Id, f.FileName);
                    }
                }
            }
        }

        [Test]
        public void DetatchFiles()
        {
            Logger.Console.Verbose = VerboseLevel.All;

            using (SnapServer server = CreateServer())
            {
                using (SnapClient client = SnapClient.Connect(server))
                using (ClientDatabaseBase<HistorianKey, HistorianValue> db = client.GetDatabase<HistorianKey, HistorianValue>("PPA"))
                {
                    using (TreeStream<HistorianKey, HistorianValue> stream = db.Read(null, null, null))
                    {
                        Console.WriteLine(stream.Count());
                    }
                    db.DetatchFiles(db.GetAllAttachedFiles().Select(x => x.Id).ToList());
                    using (TreeStream<HistorianKey, HistorianValue> stream = db.Read(null, null, null))
                    {
                        Console.WriteLine(stream.Count());
                    }
                }
            }
        }


    }
}
