using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using openHistorian.V2.Server.Configuration;

namespace openHistorian.V2.Server.Database
{
    [TestClass()]
    public class ArchiveDatabaseEngineTest
    {
        [TestMethod]
        public void Test()
        {
            var DB = new ArchiveDatabaseEngine(TestSettings1());

            for (uint x = 0; x < 1000; x++)
            {
                Thread.Sleep(1);
                DB.WriteData(x, x, x, x);
            }
            Thread.Sleep(1000);
            DB = DB;
        }

        [TestMethod]
        public void Test2()
        {
            var DB = new ArchiveDatabaseEngine(TestSettings2());

            for (uint x = 0; x < 10000; x++)
            {
                DB.WriteData(x, x, x, x);
            }
            Thread.Sleep(2000);
            
            DB = DB;
        }

        DatabaseSettings TestSettings1()
        {
            var db = new DatabaseSettings();
            db.ArchiveWriter = new ArchiveWriterSettings();
            db.ArchiveWriter.CommitOnInterval = new TimeSpan(0, 0, 0, 0, 10);
            db.ArchiveWriter.NewFileOnInterval = new TimeSpan(0, 0, 0, 0, 100);
            return db;
        }
        DatabaseSettings TestSettings2()
        {
            var db = new DatabaseSettings();
            db.ArchiveWriter = new ArchiveWriterSettings();
            db.ArchiveWriter.CommitOnInterval = new TimeSpan(0, 0, 0, 0, 10);
            db.ArchiveWriter.NewFileOnInterval = new TimeSpan(0, 0, 0, 0, 100);
            var roll = new ArchiveRolloverSettings();
            roll.Initializer.IsMemoryArchive = true;
            roll.NewFileOnInterval = new TimeSpan(0, 0, 0, 1, 0);
            roll.NewFileOnSize = long.MaxValue;
            db.ArchiveRollovers.Add(roll);

            return db;
        }


    }
}
