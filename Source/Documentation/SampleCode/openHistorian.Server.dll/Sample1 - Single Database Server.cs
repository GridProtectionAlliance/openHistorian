using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using openHistorian;
using openHistorian.Engine;

namespace SampleCode.openHistorian.Server.dll
{
    [TestFixture]
    public class Sample1
    {
        [Test]
        public void CreateScadaDatabase()
        {
            Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);

            var serverOptions = new HistorianServerOptions();
            serverOptions.IsNetworkHosted = false;
            serverOptions.IsReadOnly = false;
            serverOptions.Paths.Add(@"c:\temp\Scada\");
            
            using (var server = new HistorianServer(serverOptions))
            {
                var database = server.GetDatabase();

                for (ulong x = 0; x < 10000; x++)
                    database.Write(x, 0, 0, 0);

                database.HardCommit();
            }
        }
        
        [Test]
        public void TestReadData()
        {
            var serverOptions = new HistorianServerOptions();
            serverOptions.IsNetworkHosted = true;
            serverOptions.IsReadOnly = true;
            serverOptions.NetworkPort = 1234;
            serverOptions.Paths.Add(@"c:\temp\Scada\");

            using (var server = new HistorianServer(serverOptions))
            {
                var database = server.GetDatabase();
                using (var reader = database.OpenDataReader())
                {
                    var stream = reader.Read(0, 100);
                    stream.Cancel();
                }
            }
        }
    }
}
