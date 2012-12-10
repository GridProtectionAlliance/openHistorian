using NUnit.Framework;
using openHistorian.Server;
using openHistorian.Server.Database;

namespace openHistorian.Local
{
    [TestFixture]
    public class HistorianServer_Test1
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
                engine.Add("default", new ArchiveDatabaseEngine(WriterOptions.IsMemoryOnly()));
            }
        }

        [Test]
        public void TestMemoryAddPoints()
        {
            using (var engine = new HistorianServer())
            {
                engine.Add("default", new ArchiveDatabaseEngine(WriterOptions.IsMemoryOnly()));

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
        }

        [Test]
        public void TestOnlyReader()
        {
            using (var engine = new HistorianServer())
            {
                engine.Add("default", new ArchiveDatabaseEngine(WriterOptions.IsMemoryOnly()));

                using (var db = engine.ConnectToDatabase("dEfAuLt"))
                {
                    using (var dbr = db.OpenDataReader())
                    {
                        Assert.IsTrue(dbr.Read(0, 1000).Count() == 0);
                    }
                }
            }
        }

    }

    static class Extensions
    {
        public static int Count(this IPointStream stream)
        {
            int x = 0;
            ulong v1, v2, v3, v4;
            while (stream.Read(out v1, out v2, out v3, out v4))
                x++;
            return x;
        }
    }
}
