using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using openHistorian.Server.Configuration;

namespace openHistorian.Server.Database
{
    [TestFixture()]
    public class ArchiveDatabaseEngineTest
    {
        [Test]
        public void Test()
        {
            var DB = new ArchiveDatabaseEngine(TestSettings1());

            for (uint x = 0; x < 1000; x++)
            {
                Thread.Sleep(1);
                DB.Write(x, x, x, x);
            }
            Thread.Sleep(1000);

            using (var qry = DB.OpenDataReader())
            {
                var rdr = qry.Read(0, 1000);
                ulong cnt = 0;
                ulong k1, k2, v1, v2;
                while (rdr.Read(out k1, out k2, out v1, out v2))
                {
                    Assert.AreEqual(cnt, k1);
                    Assert.AreEqual(cnt, k2);
                    Assert.AreEqual(cnt, v1);
                    Assert.AreEqual(cnt, v2);
                    cnt++;
                }
                Assert.AreEqual(cnt, 1000ul);
            }
        }

        [Test]
        public void Test2()
        {
            var DB = new ArchiveDatabaseEngine(TestSettings2());

            for (uint x = 0; x < 10000; x++)
            {
                DB.Write(x, x, x, x);
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

        [Test]
        public void TestReader()
        {
            const ulong BigValue = uint.MaxValue + 2345243ul;

            var DB = new ArchiveDatabaseEngine(TestSettings1());
            var rand = new Random();

            for (ulong x = 0; x < 5000; x++)
            {
                Thread.Sleep(rand.Next(2));
                DB.Write(x, x * x, x * x * x, x * x * x * x);
            }
            DB.Write(BigValue, 2 * BigValue, 3 * BigValue, 4 * BigValue);
            Thread.Sleep(1000);

            using (var qry = DB.OpenDataReader())
            {
                var rdr = qry.Read(0, 4999);
                ulong cnt = 0;
                ulong k1, k2, v1, v2;
                while (rdr.Read(out k1, out k2, out v1, out v2))
                {
                    Assert.AreEqual(cnt, k1);
                    Assert.AreEqual(cnt * cnt, k2);
                    Assert.AreEqual(cnt * cnt * cnt, v1);
                    Assert.AreEqual(cnt * cnt * cnt * cnt, v2);
                    cnt++;
                }
                Assert.AreEqual(cnt, 5000ul);


                rdr = qry.Read(3000, 4000);
                cnt = 3000;
                while (rdr.Read(out k1, out k2, out v1, out v2))
                {
                    Assert.AreEqual(cnt, k1);
                    Assert.AreEqual(cnt * cnt, k2);
                    Assert.AreEqual(cnt * cnt * cnt, v1);
                    Assert.AreEqual(cnt * cnt * cnt * cnt, v2);
                    cnt++;
                }
                Assert.AreEqual(cnt, 4001ul);

                rdr = qry.Read(1245);
                cnt = 1245;
                while (rdr.Read(out k1, out k2, out v1, out v2))
                {
                    Assert.AreEqual(cnt, k1);
                    Assert.AreEqual(cnt * cnt, k2);
                    Assert.AreEqual(cnt * cnt * cnt, v1);
                    Assert.AreEqual(cnt * cnt * cnt * cnt, v2);
                    cnt++;
                }
                Assert.AreEqual(cnt, 1246ul);

                //Test BitArray
                var lst = new List<ulong>();

                for (ulong x = 100; x < 200; x += 2)
                {
                    lst.Add(x * x);
                }

                rdr = qry.Read(0, 100000, lst);
                cnt = 0;
                while (rdr.Read(out k1, out k2, out v1, out v2))
                {
                    ulong value = (ulong)Math.Sqrt(lst[(int)cnt]);
                    Assert.AreEqual(value, k1);
                    Assert.AreEqual(value * value, k2);
                    Assert.AreEqual(value * value * value, v1);
                    Assert.AreEqual(value * value * value * value, v2);
                    cnt++;
                }
                Assert.AreEqual(cnt, (ulong)lst.Count);

                //Test uint dict
                lst.Clear();

                for (ulong x = 1000; x < 2000; x += 2)
                {
                    lst.Add(x * x);
                }

                rdr = qry.Read(0, ulong.MaxValue, lst);
                cnt = 0;
                while (rdr.Read(out k1, out k2, out v1, out v2))
                {
                    ulong value = (ulong)Math.Sqrt(lst[(int)cnt]);
                    Assert.AreEqual(value, k1);
                    Assert.AreEqual(value * value, k2);
                    Assert.AreEqual(value * value * value, v1);
                    Assert.AreEqual(value * value * value * value, v2);
                    cnt++;
                }
                Assert.AreEqual(cnt, (ulong)lst.Count);

                //Test uint dict
                lst.Add(2 * BigValue);

                rdr = qry.Read(0, ulong.MaxValue, lst);
                cnt = 0;
                while (rdr.Read(out k1, out k2, out v1, out v2))
                {
                    if (cnt == (uint)lst.Count - 1)
                    {
                        ulong value = lst[(int)cnt] / 2;
                        Assert.AreEqual(value, k1);
                        Assert.AreEqual(value * 2, k2);
                        Assert.AreEqual(value * 3, v1);
                        Assert.AreEqual(value * 4, v2);
                        cnt++;
                    }
                    else
                    {
                        ulong value = (ulong)Math.Sqrt(lst[(int)cnt]);
                        Assert.AreEqual(value, k1);
                        Assert.AreEqual(value * value, k2);
                        Assert.AreEqual(value * value * value, v1);
                        Assert.AreEqual(value * value * value * value, v2);
                        cnt++;
                    }

                }
                Assert.AreEqual(cnt, (ulong)lst.Count);
            }
        }

    }
}
