using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using openHistorian.Server;
using openHistorian.Server.Database;
using openHistorian.Archive;

namespace openHistorian.Local
{
    [TestFixture]
    public class HistorianServer_Test2
    {

        [Test]
        public void TestConstructor()
        {
            using (var engine = new HistorianServer())
            {
            }
        }

        [Test]
        public void TestConfigMemory()
        {
            using (var engine = new HistorianServer())
            {
                engine.Add("default", new ArchiveDatabaseEngine(WriterOptions.IsFileBased()));
            }
        }

        [Test]
        public void TestMemoryAddPoints()
        {
            foreach (var file in Directory.GetFiles("C:\\temp\\", "*.d2"))
                File.Delete(file);

            using (var engine = new HistorianServer())
            {
                engine.Add("default", new ArchiveDatabaseEngine(WriterOptions.IsFileBased(),"c:\\temp\\"));

                using (var db = engine.ConnectToDatabase("dEfAuLt"))
                {
                    for (uint x = 0; x < 1000; x++)
                    {
                        db.Write(x, 0, 0, 0);
                    }
                    db.Commit();
                    using (var dbr = db.OpenDataReader())
                    {
                        Assert.IsTrue(dbr.Read(0, 1000).Count() == 1000);
                        Assert.IsTrue(dbr.Read(5, 25).Count() == 21);
                        var rdr = dbr.Read(900, 2000);

                        for (uint x = 1000; x < 2001; x++)
                        {
                            db.Write(x, 0, 0, 0);
                        }
                        db.Commit();

                        Assert.IsTrue(rdr.Count() == 100);
                    }
                    using (var dbr = db.OpenDataReader())
                    {

                        Assert.IsTrue(dbr.Read(900, 2000).Count() == 1101);
                    }
                }
            }

            using (var engine = new HistorianServer())
            {
                engine.Add("default", new ArchiveDatabaseEngine(WriterOptions.IsFileBased(), "c:\\temp\\"));

                using (var db = engine.ConnectToDatabase("dEfAuLt"))
                using (var dbr = db.OpenDataReader())
                {
                    Assert.IsTrue(dbr.Read(900, 2000).Count() == 1101);
                }
            }
        }

        [Test]
        public void TestOnlyReader()
        {
            using (var engine = new HistorianServer())
            {
                engine.Add("default", new ArchiveDatabaseEngine((WriterOptions?)null));
       
                using (var db = engine.ConnectToDatabase("dEfAuLt"))
                {
                    using (var dbr = db.OpenDataReader())
                    {
                        Assert.IsTrue(dbr.Read(0, 1000).Count() == 0);
                    }
                }
            }
        }

        [Test]
        public void TestOnlyReaderSpecifyFile()
        {
            string file = "c:\\temp\\archiveOne.d2";
            CreateFile(file, 10, 100, 10);

            using (var engine = new HistorianServer())
            {
                engine.Add("default", new ArchiveDatabaseEngine(null,file));
                
                using (var db = engine.ConnectToDatabase("dEfAuLt"))
                using (var dbr = db.OpenDataReader())
                {
                    Assert.AreEqual(10, dbr.Read(0, 1000).Count());
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

            using (var engine = new HistorianServer())
            {
                engine.Add("default", new ArchiveDatabaseEngine(null, file1,file2));
                
                using (var db = engine.ConnectToDatabase("dEfAuLt"))
                using (var dbr = db.OpenDataReader())
                {
                    Assert.AreEqual(20, dbr.Read(0, 1000).Count());
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

            using (var engine = new HistorianServer())
            {
                engine.Add("default", new ArchiveDatabaseEngine(null, "c:\\temp\\"));

                using (var db = engine.ConnectToDatabase("dEfAuLt"))
                using (var dbr = db.OpenDataReader())
                {
                    Assert.AreEqual(20, dbr.Read(0, 1000).Count());
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
