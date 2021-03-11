using System;
using GSF.Snap.Filters;
using NUnit.Framework;
using openHistorian.Snap;

namespace openHistorian.Data
{
    [TestFixture]
    public class PeriodicScannerTest
    {
        [Test]
        public void Test1()
        {
            DateTime start = DateTime.Now.Date;
            DateTime stop = start.AddDays(1);

            PeriodicScanner scanner = new PeriodicScanner(30);
            _ = scanner.GetParser(start, stop, 2592000);
            _ = scanner.GetParser(start, stop, 2592000 / 2);
            _ = scanner.GetParser(start, stop, 2592000 / 3);
            _ = scanner.GetParser(start, stop, 2592000 / 4);
            _ = scanner.GetParser(start, stop, 2592000 / 5);
            _ = new DateTime(634794697200000000).ToString();
        }
    }
}