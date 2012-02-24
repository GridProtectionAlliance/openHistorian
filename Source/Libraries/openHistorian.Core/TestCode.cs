using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian
{
    public class TestProcedures
    {
        public static void Test()
        {
            Test2();
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Batch;
            //Historian.StorageSystem.File.ArchiveConstants.Test();
            //Historian.StorageSystem.File.DiskIOMemoryStreamTest.Test();
            //Historian.StorageSystem.File.DiskIOUnbufferedTest.Test();
            //Historian.StorageSystem.File.FileMetaDataTest.Test();
            //Historian.StorageSystem.File.FileAllocationTableTest.Test();
            //Historian.StorageSystem.File.IndexMapperTest.Test();
            //Historian.StorageSystem.File.IndexParserTest.Test();
            //Historian.StorageSystem.File.ShadowCopyAllocatorTest.Test();
            //Historian.StorageSystem.File.ArchiveFileStreamTest.Test();
            //Historian.StorageSystem.File.TransactionalEditTest.Test();
            //Historian.StorageSystem.File.FileSystemSnapshotServiceTest.Test();

            Historian.CompressionTest.Test();

            Historian.PooledMemoryStreamTest.Test();
            
            //Historian.StorageSystem.BinaryStreamTest.Test();

            //Historian.StorageSystem.File.BenchmarkPageSizes.Test();

            //Historian.DataWriter.TestWritingPoints.Test();


            Historian.StorageSystem.BlockSorter.NodeHeaderTest.Test();
            Historian.StorageSystem.BlockSorter.TreeHeaderTest.Test();
            Historian.StorageSystem.BlockSorter.LeafNodeTest.Test();
            Historian.StorageSystem.BlockSorter.InternalNodeTest.Test();
            Historian.StorageSystem.BlockSorter.DataBucketTest.Test();
            Historian.StorageSystem.BlockSorter.NodeTest.Test();
            

            //Historian.StorageSystem.BlockSorter.Test.FileReaderTest.Test();

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
