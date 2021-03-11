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
            //var m = new MeasureCompression();
            //m.Test();

            GCTime GCT = new GCTime();
            //GCT.Test();
            GCT.Test2();



            //var tl = new TinyLock_Test();
            //tl.TestTinyLock_Lock();
            //tl.TestMonitor();

            //var hl = new HalfLock_Test();
            //hl.TestTinyLock_Lock();
            Console.ReadLine();

            
            //var st = new ThreadContainerBase_Test();
            ////st.TestTimed();
            //st.Test();


            //var tree = new SortedTree256Test();
            //tree.SortedTree256Archive();
            //ReadPoints.TestReadPoints2();
            //ReadPoints.ReadAllPoints();
            //ReadPoints.TestReadFilteredPoints();

            //Console.ReadLine();
        }
    }
}