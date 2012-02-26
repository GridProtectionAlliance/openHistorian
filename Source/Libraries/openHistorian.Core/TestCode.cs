using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.Core
{
    public class TestProcedures
    {
        public static void Test()
        {
            Test2();
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Batch;
            //openHistorian.Core.StorageSystem.File.ArchiveConstants.Test();
            //openHistorian.Core.StorageSystem.File.DiskIOMemoryStreamTest.Test();
            //openHistorian.Core.StorageSystem.File.DiskIOUnbufferedTest.Test();
            //openHistorian.Core.StorageSystem.File.FileMetaDataTest.Test();
            //openHistorian.Core.StorageSystem.File.FileAllocationTableTest.Test();
            //openHistorian.Core.StorageSystem.File.IndexMapperTest.Test();
            //openHistorian.Core.StorageSystem.File.IndexParserTest.Test();
            //openHistorian.Core.StorageSystem.File.ShadowCopyAllocatorTest.Test();
            //openHistorian.Core.StorageSystem.File.ArchiveFileStreamTest.Test();
            //openHistorian.Core.StorageSystem.File.TransactionalEditTest.Test();
            //openHistorian.Core.StorageSystem.File.FileSystemSnapshotServiceTest.Test();

            openHistorian.Core.CompressionTest.Test();

            openHistorian.Core.PooledMemoryStreamTest.Test();

            openHistorian.Core.StorageSystem.BinaryStreamTest.Test();

            //openHistorian.Core.StorageSystem.File.BenchmarkPageSizes.Test();

            //openHistorian.Core.DataWriter.TestWritingPoints.Test();

            //openHistorian.Core.StorageSystem.Generic.LeafNodeTest.Test();
            openHistorian.Core.StorageSystem.Generic.NodeTest.Test();


            //openHistorian.Core.StorageSystem.BlockSorter.NodeHeaderTest.Test();
            //openHistorian.Core.StorageSystem.BlockSorter.TreeHeaderTest.Test();
            //openHistorian.Core.StorageSystem.BlockSorter.LeafNodeTest.Test();
            //openHistorian.Core.StorageSystem.BlockSorter.InternalNodeTest.Test();
            //openHistorian.Core.StorageSystem.BlockSorter.DataBucketTest.Test();
            //openHistorian.Core.StorageSystem.BlockSorter.NodeTest.Test();

            //openHistorian.Core.StorageSystem.Importer.NodeHeaderTest.Test();
            //openHistorian.Core.StorageSystem.Importer.TreeHeaderTest.Test();
            //openHistorian.Core.StorageSystem.Importer.LeafNodeTest.Test();
            //openHistorian.Core.StorageSystem.Importer.InternalNodeTest.Test();
            //openHistorian.Core.StorageSystem.QuickSort.NodeTest.Test();

            //openHistorian.Core.StorageSystem.BlockSorter.Test.FileReaderTest.Test();

        }
        public static int Test2()
        {
            int[] data = new int[]{1};
            int i1, i2, i3, i4, i5;

            i1 = ~Array.BinarySearch(data, 0, data.Length, 0);
            i2 = ~Array.BinarySearch(data, 0, data.Length, 2);
            i3 = ~Array.BinarySearch(data, 0, data.Length, 4);
            i4 = ~Array.BinarySearch(data, 0, data.Length, 6);
            i5 = ~Array.BinarySearch(data, 0, data.Length, 8);

            int i6 = i1 + i2 + i3 + i4 + i5;
            return i6;
        }
    }
}
