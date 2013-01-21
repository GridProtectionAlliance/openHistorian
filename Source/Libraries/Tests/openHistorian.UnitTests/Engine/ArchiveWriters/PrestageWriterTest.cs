using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace openHistorian.Engine.ArchiveWriters
{
    [TestFixture]
    public class PrestageWriterTest
    {
        int m_pointsRead = 0;
        long m_sequenceNumber = 0;
        List<int> m_pointCount = new List<int>();

        [Test]
        public void TestOnCommitInterval()
        {
            var prestageSettings = new PrestageSettings()
            {
                DelayOnPointCount = 20 * 1000,
                RolloverPointCount = 1 * 1000,
                RolloverInterval = 1000
            };

            using (var prestage = new PrestageWriter(prestageSettings, FinalizeArchiveFile))
            {
                AddPoints(999, prestage);
                Thread.Sleep(100);
                Assert.AreEqual(0L, m_sequenceNumber);
                Assert.AreEqual(0, m_pointsRead);
                AddPoints(1, prestage);
                Thread.Sleep(100);
                Assert.AreEqual(1000L, m_sequenceNumber);
                Assert.AreEqual(1000L, m_pointsRead);
            }
            Thread.Sleep(100);
        }

        [Test]
        public void Test1()
        {

            var prestageSettings = new PrestageSettings()
            {
                DelayOnPointCount = 30 * 1000,
                RolloverPointCount = 10 * 1000,
                RolloverInterval = 1000
            };

            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var prestage = new PrestageWriter(prestageSettings, FinalizeArchiveFileList))
            {
                for (int x = 0; x < 1000; x++)
                {
                    AddPoints(10000, prestage);
                    Thread.Sleep(0);
                }
                //Thread.Sleep(10);

                //Assert.Greater(1999, m_sequenceNumber);
                //Assert.Greater(1999, m_pointsRead);
                prestage.Stop();
            }
            sw.Stop();
            Console.WriteLine(m_pointsRead);
            Thread.Sleep(1000);
        }

        [Test]
        public void Test2()
        {

            var prestageSettings = new PrestageSettings()
            {
                DelayOnPointCount = 30 * 1000,
                RolloverPointCount = 10 * 1000,
                RolloverInterval = 10
            };

            Stopwatch sw = new Stopwatch();
            sw.Start();
            using (var prestage = new PrestageWriter(prestageSettings, FinalizeArchiveFileList))
            {
                for (int x = 0; x < 1000; x++)
                {
                    AddPoints(500, prestage);
                    Thread.Sleep(1);
                }
                //Thread.Sleep(10);

                //Assert.Greater(1999, m_sequenceNumber);
                //Assert.Greater(1999, m_pointsRead);
                prestage.Stop();
            }
            sw.Stop();
            Console.WriteLine(m_pointsRead);
            Thread.Sleep(1000);
        }



        void FinalizeArchiveFile(RolloverArgs args)
        {
            m_pointsRead = args.CurrentStream.Count();
            m_sequenceNumber = args.SequenceNumber;
            Assert.Null(args.File);
        }

        void FinalizeArchiveFileList(RolloverArgs args)
        {
            m_pointCount.Add(args.CurrentStream.Count());
            m_sequenceNumber = args.SequenceNumber;
            Assert.Null(args.File);
        }

        void AddPoints(int count, PrestageWriter writer)
        {
            for (uint x = 0; x < count; x++)
            {
                writer.Write(x, 1 * x, 2 * x, 3 * x);
            }
        }
    }
}
