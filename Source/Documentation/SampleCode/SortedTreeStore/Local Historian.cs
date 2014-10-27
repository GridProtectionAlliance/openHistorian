using System;
using System.Threading;
using GSF;
using GSF.Diagnostics;
using GSF.SortedTreeStore;
using GSF.SortedTreeStore.Services;
using GSF.SortedTreeStore.Services.Configuration;
using NUnit.Framework;
using openHistorian.Collections;
using System.Linq;

namespace SampleCode.SortedTreeStore
{
    [TestFixture]
    public class Local_Historian
    {
        public Server CreateServer()
        {
            var settings = new HistorianServerDatabaseConfig("PPA", @"C:\Temp\Synchrophasor", true);
            var server = new Server(settings);
            return server;
        }

        [Test]
        public void CreateHistorian()
        {
            Logger.ReportToConsole(VerboseLevel.All);

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
            Logger.ReportToConsole(VerboseLevel.All);

            using (var server = CreateServer())
            {
                using (var client = Client.Connect(server))
                {
                    client.GetDatabase("PPA").AttachFilesOrPaths(new string[] { @"C:\Temp\Synchrophasor\Dir\File2.d2" });
                }
            }

        }

        [Test]
        public void ReadData()
        {
            Logger.ReportToConsole(VerboseLevel.All);

            using (var server = CreateServer())
            {
                using (var client = Client.Connect(server))
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
            Logger.ReportToConsole(VerboseLevel.All);

            using (var server = CreateServer())
            {
                using (var client = Client.Connect(server))
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
            Logger.ReportToConsole(VerboseLevel.All);

            using (var server = CreateServer())
            {
                using (var client = Client.Connect(server))
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
            Logger.ReportToConsole(VerboseLevel.All);

            using (var server = CreateServer())
            {
                using (var client = Client.Connect(server))
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
