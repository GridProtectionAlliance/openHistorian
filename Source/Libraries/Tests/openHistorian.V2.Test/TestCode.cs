using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.V2.Test
{
    internal static class TestCode
    {
        [STAThread]
        private static void Main()
        {
            BPlusTreeBaseTest c = new BPlusTreeBaseTest();
            c.BenchmarkGetRange();
        }
    }
}
