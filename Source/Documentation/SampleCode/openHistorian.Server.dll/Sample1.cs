using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using openHistorian.Engine;

namespace SampleCode.openHistorian.Server.dll
{
    public class Sample1
    {
        public void Test()
        {
            ArchiveDatabaseEngine engine = new ArchiveDatabaseEngine(WriterOptions.IsFileBased(), @"c:\Temp\");
            
        }
    }
}
