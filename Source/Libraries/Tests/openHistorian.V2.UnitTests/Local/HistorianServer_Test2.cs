using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using openHistorian.V2.Server.Database.Archive;

namespace openHistorian.V2.Local
{
    [TestFixture]
    public class HistorianServer_Test2
    {

        [Test]
        public void TestConstructor()
        {
            using (IHistorian engine = new HistorianServer())
            {
                engine.Manage();
            }
        }

        [Test]
        public void TestConfigMemory()
        {
            using (IHistorian engine = new HistorianServer())
            {
                var manage = engine.Manage();
                IDatabaseConfig cfg = manage.CreateConfig(WriterOptions.IsFileBased());
                engine.Manage().Add("default", cfg);
            }
        }

        [Test]
        public void TestMemoryAddPoints()
        {
            foreach (var file in Directory.GetFiles("C:\\temp\\", "*.d2"))
                File.Delete(file);

            using (IHistorian engine = new HistorianServer())
            {
                var manage = engine.Manage();
                IDatabaseConfig cfg = manage.CreateConfig(WriterOptions.IsFileBased());
                cfg.Paths.AddPath("C:\\temp\\", true);

                engine.Manage().Add("default", cfg);

                using (var db = engine.ConnectToDatabase("dEfAuLt"))
                {
                    for (uint x = 0; x < 1000; x++)
                    {
                        db.Write(x, 0, 0, 0);
                    }
                    db.CommitToDisk();

                    Assert.IsTrue(db.Read(0, 1000).Count() == 1000);
                    Assert.IsTrue(db.Read(5, 25).Count() == 21);

                    var rdr = db.Read(900, 2000);

                    for (uint x = 1000; x < 2001; x++)
                    {
                        db.Write(x, 0, 0, 0);
                    }
                    db.CommitToDisk();

                    Assert.IsTrue(rdr.Count() == 100);
                    Assert.IsTrue(db.Read(900, 2000).Count() == 1101);
                }
            }

            using (IHistorian engine = new HistorianServer())
            {
                var manage = engine.Manage();
                IDatabaseConfig cfg = manage.CreateConfig(WriterOptions.IsFileBased());
                cfg.Paths.AddPath("C:\\temp\\", true);

                engine.Manage().Add("default", cfg);

                using (var db = engine.ConnectToDatabase("dEfAuLt"))
                {
                    Assert.IsTrue(db.Read(900, 2000).Count() == 1101);
                }
            }
        }

        [Test]
        public void TestOnlyReader()
        {
            using (IHistorian engine = new HistorianServer())
            {
                var manage = engine.Manage();
                IDatabaseConfig cfg = manage.CreateConfig();
                engine.Manage().Add("default", cfg);

                using (var db = engine.ConnectToDatabase("dEfAuLt"))
                {
                    Assert.IsTrue(db.Read(0, 1000).Count() == 0);
                }
            }
        }

        [Test]
        public void TestOnlyReaderSpecifyFile()
        {
            string file = "c:\\temp\\archiveOne.d2";
            CreateFile(file, 10, 100, 10);

            using (IHistorian engine = new HistorianServer())
            {
                var manage = engine.Manage();
                IDatabaseConfig cfg = manage.CreateConfig();
                cfg.Paths.AddPath(file, false);
                engine.Manage().Add("default", cfg);

                using (var db = engine.ConnectToDatabase("dEfAuLt"))
                {
                    Assert.AreEqual(10, db.Read(0, 1000).Count());
                }
            }
        }

        [Test]
        public void TestOnlyReaderMultipleFiles()
        {
            string file1 = "c:\\temp\\archiveOne.d2";
            string file2 = "c:\\temp\\archiveTwo.d2";
            CreateFile(file1, 10, 100, 10);
            CreateFile(file2, 11, 101, 10);

            using (IHistorian engine = new HistorianServer())
            {
                var manage = engine.Manage();
                IDatabaseConfig cfg = manage.CreateConfig();
                cfg.Paths.AddPath(file1, false);
                cfg.Paths.AddPath(file2, false);
                engine.Manage().Add("default", cfg);

                using (var db = engine.ConnectToDatabase("dEfAuLt"))
                {
                    Assert.AreEqual(20, db.Read(0, 1000).Count());
                }
            }
        }

        [Test]
        public void TestOnlyReaderFolder()
        {
            Array.ForEach(Directory.GetFiles("c:\\temp\\", "*.d2"), File.Delete);

            string file1 = "c:\\temp\\archiveOne.d2";
            string file2 = "c:\\temp\\archiveTwo.d2";
            CreateFile(file1, 10, 100, 10);
            CreateFile(file2, 11, 101, 10);

            using (IHistorian engine = new HistorianServer())
            {
                var manage = engine.Manage();
                IDatabaseConfig cfg = manage.CreateConfig();
                cfg.Paths.AddPath("c:\\temp\\", false);
                engine.Manage().Add("default", cfg);

                using (var db = engine.ConnectToDatabase("dEfAuLt"))
                {
                    Assert.AreEqual(20, db.Read(0, 1000).Count());
                }
            }
        }

        void CreateFile(string name, ulong start, ulong stop, ulong step)
        {
            if (File.Exists(name))
                File.Delete(name);

            using (var af = ArchiveFile.CreateFile(name))
            {
                using (var edit = af.BeginEdit())
                {
                    for (ulong x = start; x <= stop; x += step)
                        edit.AddPoint(x, x, x, x);
                    edit.Commit();
                }
                af.Dispose();
            }
        }
    }

}
