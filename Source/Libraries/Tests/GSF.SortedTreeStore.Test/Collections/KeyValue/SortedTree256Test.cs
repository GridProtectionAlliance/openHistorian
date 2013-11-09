//using System;
//using System.IO;
//using NUnit.Framework;
//using openHistorian.Archive;
//using openHistorian.FileStructure;
//using GSF.IO.Unmanaged;

//namespace openHistorian.Collections
//{
//    [TestFixture]
//    public class SortedTree256Test
//    {
//        const uint Count = 10000;

//        [Test]
//        public void SortedTree256ArchiveFileDelta()
//        {
//            if (File.Exists(("c:\\temp\\ArchiveFileDelMe.d2")))
//                File.Delete("c:\\temp\\ArchiveFileDelMe.d2");
//            using (TransactionalFileStructure file = TransactionalFileStructure.CreateFile("c:\\temp\\ArchiveFileDelMe.d2", 4096))
//            using (var edit = file.BeginEdit())
//            using (var stream = edit.CreateFile(SubFileName.Empty))
//            using (BinaryStream bs = new BinaryStream(stream))
//            {
//                SortedTree256BaseTest.BenchmarkTree(() => SortedTree256.Create(bs, 4096 - FileStructureConstants.BlockFooterLength, CompressionMethod.DeltaEncoded), Count);
//            }
//            File.Delete("c:\\temp\\ArchiveFileDelMe.d2");
//        }

//        [Test]
//        public void SortedTree256ArchiveFileTS()
//        {
//            if (File.Exists(("c:\\temp\\ArchiveFileDelMe.d2")))
//                File.Delete("c:\\temp\\ArchiveFileDelMe.d2");
//            using (TransactionalFileStructure file = TransactionalFileStructure.CreateFile("c:\\temp\\ArchiveFileDelMe.d2", 4096))
//            using (var edit = file.BeginEdit())
//            using (var stream = edit.CreateFile(SubFileName.Empty))
//            using (BinaryStream bs = new BinaryStream(stream))
//            {
//                SortedTree256BaseTest.BenchmarkTree(() => SortedTree256.Create(bs, 4096 - FileStructureConstants.BlockFooterLength, CompressionMethod.TimeSeriesEncoded), Count);
//            }
//            File.Delete("c:\\temp\\ArchiveFileDelMe.d2");
//        }

//        [Test]
//        public void SortedTree256ArchiveFile()
//        {
//            if (File.Exists(("c:\\temp\\ArchiveFileDelMe.d2")))
//                File.Delete("c:\\temp\\ArchiveFileDelMe.d2");

//            using (TransactionalFileStructure file = TransactionalFileStructure.CreateFile("c:\\temp\\ArchiveFileDelMe.d2", 4096))
//            using (var edit = file.BeginEdit())
//            using (var stream = edit.CreateFile(SubFileName.Empty))
//            using (BinaryStream bs = new BinaryStream(stream))
//            {
//                SortedTree256BaseTest.BenchmarkTree(() => SortedTree256.Create(bs, 4096 - FileStructureConstants.BlockFooterLength), Count);
//                SortedTree256BaseEnhancedTest.BenchmarkTreeScanner(() =>
//                {
//                    var tree = SortedTree256.Create(bs, 4096);
//                    tree.SkipIntermediateSaves = true;
//                    return tree;
//                }, Count);
//            }
//            File.Delete("c:\\temp\\ArchiveFileDelMe.d2");
//        }

//        [Test]
//        public void SortedTree256Archive()
//        {
//            using (TransactionalFileStructure file = TransactionalFileStructure.CreateInMemory(4096))
//            using (var edit = file.BeginEdit())
//            using (var stream = edit.CreateFile(SubFileName.Empty))
//            using (BinaryStream bs = new BinaryStream(stream))
//            {
//                SortedTree256BaseTest.BenchmarkTree(() => SortedTree256.Create(bs, 4096 - FileStructureConstants.BlockFooterLength), Count);
//            }
//            Console.WriteLine((Statistics.ChecksumCount / 11).ToString("N0"));
//        }

//        [Test]
//        public void SortedTree256Method()
//        {
//            using (BinaryStream bs = new BinaryStream())
//            {
//                SortedTree256BaseTest.BenchmarkTree(() => SortedTree256.Create(bs, 4096), Count);
//            }
//        }

//        [Test]
//        public void SortedTree256Delta()
//        {
//            using (BinaryStream bs = new BinaryStream())
//            {
//                SortedTree256BaseTest.BenchmarkTree(() => SortedTree256.Create(bs, 4096, CompressionMethod.DeltaEncoded), Count);
//            }
//        }

//        [Test]
//        public void SortedTree256TS()
//        {
//            using (BinaryStream bs = new BinaryStream())
//            {
//                SortedTree256BaseTest.BenchmarkTree(() => SortedTree256.Create(bs, 4096, CompressionMethod.TimeSeriesEncoded), Count);
//                SortedTree256BaseEnhancedTest.BenchmarkTreeScanner(() => SortedTree256.Create(bs, 4096, CompressionMethod.TimeSeriesEncoded), Count);
//            }
//        }
//    }
//}

