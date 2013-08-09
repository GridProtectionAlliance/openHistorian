using System;
using NUnit.Framework;

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
            QueryFilterTimestamp parser = scanner.GetParser(start, stop, 2592000);
            parser = scanner.GetParser(start, stop, 2592000 / 2);
            parser = scanner.GetParser(start, stop, 2592000 / 3);
            parser = scanner.GetParser(start, stop, 2592000 / 4);
            parser = scanner.GetParser(start, stop, 2592000 / 5);
            string str = new DateTime(634794697200000000).ToString();
        }
    }
}