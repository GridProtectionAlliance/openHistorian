using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using openHistorian.Collections;
using openHistorian.Collections.Generic;
using openHistorian.Engine.Configuration;

namespace openHistorian.Engine.ArchiveWriters
{
    [TestFixture]
    class StagingFileTest
    {
        [Test]
        public void Test1()
        {
            using (var list = new ArchiveList<HistorianKey, HistorianValue>())
            using (var files = list.CreateNewClientResources())
            {
                var settings = new ArchiveInitializerSettings();
                settings.IsMemoryArchive = true;
                var initializer = new ArchiveInitializer<HistorianKey, HistorianValue>(settings, CreateFixedSizeNode.TypeGuid);

                var stage = new StagingFile<HistorianKey, HistorianValue>(list, initializer);
                files.UpdateSnapshot();

                Assert.AreEqual(0L, stage.Size);

                Assert.AreEqual(0, files.Tables.Length);

                stage.Append(new PointStreamSequential(1, 2));
                Assert.AreEqual(0, files.Tables.Length);
                files.UpdateSnapshot();
                Assert.AreEqual(1, files.Tables.Length);
                using (var read = files.Tables[0].ActiveSnapshotInfo.CreateReadSnapshot())
                {
                    var scan = read.GetTreeScanner();
                    scan.SeekToStart();
                    Assert.AreEqual(2, scan.Count());
                }
                Assert.Less(8L * 1024, stage.Size);
                var file = stage.GetFileAndSetNull();
                Assert.AreEqual(0L, stage.Size);
                files.UpdateSnapshot();
                Assert.AreEqual(1, files.Tables.Length);

                stage.Combine(file);

                files.UpdateSnapshot();
                Assert.AreEqual(1, files.Tables.Length);

                using (var read = files.Tables[0].ActiveSnapshotInfo.CreateReadSnapshot())
                {
                    var scan = read.GetTreeScanner();
                    scan.SeekToStart();
                    Assert.AreEqual(2, scan.Count());
                }
            }
        }

    }
}
