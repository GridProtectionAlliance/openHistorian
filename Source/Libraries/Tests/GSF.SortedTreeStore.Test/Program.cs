using System;
using System.IO;
using System.Windows.Forms;

namespace winformsVisN
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {

            Array.ForEach(Directory.GetFiles(@"c:\temp\Scada\", "*.d2", SearchOption.AllDirectories), File.Delete);

            //ConvertArchiveFile.ConvertVersion1File("C:\\Unison\\GPA\\ArchiveFiles\\archive1_archive_2012-07-26 15!35!36.166_to_2012-07-26 15!40!36.666.d",
            //    "C:\\Temp\\Scada\\Archive.d2",
            //    //CreateFixedSizeNode.TypeGuid, 10000000);
            //    HistorianFileEncodingDefinition.TypeGuid, 10000000);


            //if (File.Exists("C:\\Unison\\GPA\\ArchiveFiles\\archive1_archive_2012-07-26 15!35!36.166_to_2012-07-26 15!40!36.666.d4"))
            //    File.Delete("C:\\Unison\\GPA\\ArchiveFiles\\archive1_archive_2012-07-26 15!35!36.166_to_2012-07-26 15!40!36.666.d4");



            //ConvertArchiveFile.ConvertVersion1File("C:\\Unison\\GPA\\ArchiveFiles\\archive1_archive_2012-07-26 15!35!36.166_to_2012-07-26 15!40!36.666.d",
            //    "C:\\Unison\\GPA\\ArchiveFiles\\archive1_archive_2012-07-26 15!35!36.166_to_2012-07-26 15!40!36.666.d4",
            //    CreateHistorianCompressionTs.TypeGuid, 1000000);

            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine(HistorianCompressionTsScanner.Stage1.ToString());
            //sb.AppendLine(HistorianCompressionTsScanner.Stage2.ToString());
            //sb.AppendLine(HistorianCompressionTsScanner.Stage3.ToString());
            //sb.AppendLine(HistorianCompressionTsScanner.Stage4.ToString());
            //sb.AppendLine(HistorianCompressionTsScanner.Stage5.ToString());
            //Clipboard.SetText(sb.ToString());
            return;
            //ConvertArchiveFile.ConvertVersion1File("C:\\Unison\\GPA\\ArchiveFiles\\p1_archive_2012-04-14 09!21!08.800_to_2012-04-14 13!29!47.400.d",
            //    "C:\\Unison\\GPA\\ArchiveFiles\\p1_archive_2012-04-14 09!21!08.800_to_2012-04-14 13!29!47.400.d2",
            //    CreateHistorianCompressionDelta.TypeGuid, 1000000);

            //ConvertArchiveFile.ConvertVersion1File("C:\\Unison\\GPA\\ArchiveFiles\\p1_archive_2012-04-14 09!21!08.800_to_2012-04-14 13!29!47.400.d",
            //    "C:\\Unison\\GPA\\ArchiveFiles\\p1_archive_2012-04-14 09!21!08.800_to_2012-04-14 13!29!47.400.d1",
            //    CreateFixedSizeNode.TypeGuid, 1000000);



            //OptimizeCompressionMethodTest.Run3();
            return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //var c = new SortedTree64Test();
            //c.TestSortedTree(10000,true);
        }
    }
}