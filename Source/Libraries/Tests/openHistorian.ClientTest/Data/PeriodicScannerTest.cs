using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using openHistorian.Data;

namespace openHistorian.Data
{
    [TestFixture]
    public class PeriodicScannerTest
    {
        [Test]
        public void Test1()
        {
            var start = DateTime.Now.Date;
            var stop = start.AddDays(1);
                
            var scanner = new PeriodicScanner(30);
            var parser = scanner.GetParser(start, stop, 2592000);
            parser = scanner.GetParser(start, stop, 2592000 / 2);
            parser = scanner.GetParser(start, stop, 2592000 / 3);
            parser = scanner.GetParser(start, stop, 2592000 / 4);
            parser = scanner.GetParser(start, stop, 2592000 / 5);
            var str = new DateTime(634794697200000000).ToString();

        }
    }
}
