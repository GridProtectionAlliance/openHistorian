using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using openHistorian.Engine.Configuration;

namespace openHistorian.Engine.ArchiveWriters
{
    [TestFixture]
    public class StageWriterTest
    {
        [Test]
        public void Test1()
        {
            using (var list = new ArchiveList())
            using (var files = list.CreateNewClientResources())
            {
                var settings = new ArchiveInitializerSettings();
                settings.IsMemoryArchive = true;
                var initializer = new ArchiveInitializer(settings);

                var stage = new StagingFile(list, initializer);

                var ss = new StageWriterSettings
                    {
                        RolloverInterval = 100,
                        RolloverSize = 1024 * 1024 * 1024,
                        StagingFile = stage
                    };

                using (var sw = new StageWriter(ss, FinalizeArchiveFile))
                {
                    sw.AppendData(new RolloverArgs(null, new PointStreamSequential(1, 20), 20));
                    sw.Stop();
                }
            }
        }

        List<int> m_pointCount = new List<int>();
        int m_pointsRead = 0;
        long m_sequenceNumber = 0;

        void FinalizeArchiveFile(RolloverArgs args)
        {
            m_pointsRead = (int)args.File.Count();
            m_sequenceNumber = args.SequenceNumber;
            Assert.NotNull(args.File);
        }
    }
}
