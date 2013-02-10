using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using openHistorian.FileStructure;
using GSF.IO.Unmanaged;
using openHistorian.FileStructure.IO;

namespace openHistorian.Collections.KeyValue
{
    [TestFixture]
    public class SortedTree256Test
    {
        const uint Count = 10000;

        [Test]
        public void SortedTree256ArchiveFileDelta()
        {
            using (TransactionalFileStructure file = TransactionalFileStructure.CreateFile("c:\\temp\\ArchiveFileDelMe.d2", 4096))
            using (var edit = file.BeginEdit())
            using (var stream = edit.CreateFile(Guid.NewGuid(), 12))
            using (BinaryStream bs = new BinaryStream(stream))
            {
                SortedTree256BaseTest.BenchmarkTree(() => new SortedTree256DeltaEncoded(bs, 4096), Count);
            }
            File.Delete("c:\\temp\\ArchiveFileDelMe.d2");
        }

        [Test]
        public void SortedTree256ArchiveFileTS()
        {
            using (TransactionalFileStructure file = TransactionalFileStructure.CreateFile("c:\\temp\\ArchiveFileDelMe.d2", 4096))
            using (var edit = file.BeginEdit())
            using (var stream = edit.CreateFile(Guid.NewGuid(), 12))
            using (BinaryStream bs = new BinaryStream(stream))
            {
                SortedTree256BaseTest.BenchmarkTree(() => new SortedTree256TSEncoded(bs, 4096), Count);
            }
            File.Delete("c:\\temp\\ArchiveFileDelMe.d2");
        }
        
        [Test]
        public void SortedTree256ArchiveFile()
        {
            using (TransactionalFileStructure file = TransactionalFileStructure.CreateFile("c:\\temp\\ArchiveFileDelMe.d2",4096))
            using (var edit = file.BeginEdit())
            using (var stream = edit.CreateFile(Guid.NewGuid(), 12))
            using (BinaryStream bs = new BinaryStream(stream))
            {
                SortedTree256BaseTest.BenchmarkTree(() => new SortedTree256(bs, 4096), Count);
                SortedTree256BaseEnhancedTest.BenchmarkTreeScanner(() =>
                {
                    var tree = new SortedTree256TSEncoded(bs, 4096);
                    tree.SkipIntermediateSaves = true;
                    return tree;
                }, Count);
            }
            File.Delete("c:\\temp\\ArchiveFileDelMe.d2");
        }

        [Test]
        public void SortedTree256Archive()
        {
            using (TransactionalFileStructure file = TransactionalFileStructure.CreateInMemory(4096))
            using (var edit = file.BeginEdit())
            using (var stream = edit.CreateFile(Guid.NewGuid(), 12))
            using (BinaryStream bs = new BinaryStream(stream))
            {
                SortedTree256BaseTest.BenchmarkTree(() => new SortedTree256(bs, 4096), Count);
            }
            Console.WriteLine((Footer.ChecksumCount / 11).ToString("N0"));
        }

        [Test]
        public void SortedTree256()
        {
            using (BinaryStream bs = new BinaryStream())
            {
                SortedTree256BaseTest.BenchmarkTree(() => new SortedTree256(bs, 4096), Count);
            }
        }

        [Test]
        public void SortedTree256Delta()
        {
            using (BinaryStream bs = new BinaryStream())
            {
                SortedTree256BaseTest.BenchmarkTree(() => new SortedTree256DeltaEncoded(bs, 4096), Count);
            }
        }

        [Test]
        public void SortedTree256TS()
        {
            using (BinaryStream bs = new BinaryStream())
            {
                SortedTree256BaseTest.BenchmarkTree(() => new SortedTree256TSEncoded(bs, 4096), Count);
                SortedTree256BaseEnhancedTest.BenchmarkTreeScanner(() => new SortedTree256TSEncoded(bs, 4096), Count);
            }
        }
    }
}
