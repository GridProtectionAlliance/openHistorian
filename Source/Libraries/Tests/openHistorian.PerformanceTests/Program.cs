using System;

namespace openHistorian.PerformanceTests
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var m = new MeasureCompression();
            m.Test();


            //var tree = new SortedTree256Test();
            //tree.SortedTree256Archive();
            //ReadPoints.TestReadPoints2();
            //ReadPoints.ReadAllPoints();
            //ReadPoints.TestReadFilteredPoints();
            
            //Console.ReadLine();
        }
    }
}