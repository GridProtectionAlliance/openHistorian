using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using openHistorian.V2.Collections.KeyValue;

namespace openHistorian.V2.PerformanceTests
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var tree = new SortedTree256Test();
            tree.SortedTree256Archive();
            Console.ReadLine();
        }
    }
}
