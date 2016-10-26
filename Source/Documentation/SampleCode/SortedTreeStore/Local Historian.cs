using System;
using System.Threading;
using GSF;
using GSF.Diagnostics;
using GSF.Snap;
using GSF.Snap.Services;
using GSF.Snap.Services.Configuration;
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
            var settings = new HistorianServerDatabaseConfig("PPA", @"C:\Temp\Synchrophasor", true);
            var server = new SnapServer(settings);
            return server;
        }

        [Test]
        public void CreateHistorian()
        {
            Logger.Console.Verbose = VerboseLevel.All;

            using (var server = CreateServer())
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

            using (var server = CreateServer())
            {
                using (var client = SnapClient.Connect(server))
                {
                    client.GetDatabase("PPA").AttachFilesOrPaths(new string[] { @"C:\Temp\Synchrophasor\Dir\File2.d2" });
                }
            }

        }

        [Test]
        public void ReadData()
        {
            Logger.Console.Verbose = VerboseLevel.All;

            using (var server = CreateServer())
            {
                using (var client = SnapClient.Connect(server))
                using (var db = client.GetDatabase<HistorianKey, HistorianValue>("PPA"))
                using (var stream = db.Read(null, null, null))
                {
                    Console.WriteLine(stream.Count());
                }
            }
        }

        [Test]
        public void WriteData()
        {
            Logger.Console.Verbose = VerboseLevel.All;

            using (var server = CreateServer())
            {
                using (var client = SnapClient.Connect(server))
                using (var db = client.GetDatabase<HistorianKey, HistorianValue>("PPA"))
                {
                    var key = new HistorianKey();
                    var value = new HistorianValue();
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

            using (var server = CreateServer())
            {
                using (var client = SnapClient.Connect(server))
                using (var db = client.GetDatabase("PPA"))
                {
                    foreach (var f in db.GetAllAttachedFiles())
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

            using (var server = CreateServer())
            {
                using (var client = SnapClient.Connect(server))
                using (var db = client.GetDatabase<HistorianKey, HistorianValue>("PPA"))
                {
                    using (var stream = db.Read(null, null, null))
                    {
                        Console.WriteLine(stream.Count());
                    }
                    db.DetatchFiles(db.GetAllAttachedFiles().Select(x => x.Id).ToList());
                    using (var stream = db.Read(null, null, null))
                    {
                        Console.WriteLine(stream.Count());
                    }
                }
            }
        }


    }
}
