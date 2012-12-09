using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using openHistorian.Server.Database.Archive;

namespace openHistorian.Local
{
    [TestFixture]
    public class HistorianServer_Test1
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
                IDatabaseConfig cfg = manage.CreateConfig(WriterOptions.IsMemoryOnly());
                engine.Manage().Add("default", cfg);
            }
        }

        [Test]
        public void TestMemoryAddPoints()
        {
            using (IHistorian engine = new HistorianServer())
            {
                var manage = engine.Manage();
                IDatabaseConfig cfg = manage.CreateConfig(WriterOptions.IsMemoryOnly());
                engine.Manage().Add("default", cfg);

                using (var db = engine.ConnectToDatabase("dEfAuLt"))
                {
                    for (uint x = 0; x < 1000; x++)
                    {
                        db.Write(x, 0, 0, 0);
                    }
                    db.Commit();
                    Assert.IsTrue(db.Read(0, 1000).Count() == 1000);
                    Assert.IsTrue(db.Read(5, 25).Count() == 21);

                    var rdr = db.Read(900, 2000);

                    for (uint x = 1000; x < 2001; x++)
                    {
                        db.Write(x, 0, 0, 0);
                    }
                    db.Commit();

                    Assert.IsTrue(rdr.Count() == 100);
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
