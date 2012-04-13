using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace openHistorian.V2
{
    public class TestProcedures
    {
        public static void Test()
        {
            //Unmanaged.BufferPool.SetMinimumMemoryUsage(Unmanaged.BufferPool.MaximumMemoryUsage);
            System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Batch;
            //Test2();
            //return;
            //Unmanaged.Generic.NodeTestPageSize.Test();

            //Unmanaged.BinaryStreamBenchmark.Run();
            //return;

            //Unmanaged.Generic.NodeTestHugeSequential.Test();
            return;

            //ArchiveTest.Test();
            //return;
            //Unmanaged.BitArrayTest.Test();
            //Unmanaged.MemoryTest.Test();

            //Unmanaged.BufferPoolTest.Test();

            //Unmanaged.MemoryStreamTest.Test();
            Unmanaged.BinaryStreamTest.Test();
            //Unmanaged.BufferedStreamTest.Test();

            //Unmanaged.BufferedStreamTest.BenchmarkTest();

            //Unmanaged.Specialized.NodeTest.Test();

            //Unmanaged.Generic.NodeTest.Test();
            //Unmanaged.Generic.NodeTest2.Test();

            //Unmanaged.Generic.NodeTestStack.Test();

            //StorageSystem.File.DiskIOEnhanced2Test.Test();

            //StorageSystem.File.FileMetaDataTest.Test();
            //StorageSystem.File.FileAllocationTableTest.Test();
            //StorageSystem.File.IndexMapperTest.Test();
            //StorageSystem.File.IndexParserTest.Test();
            //StorageSystem.File.ShadowCopyAllocatorTest.Test();
            //StorageSystem.File.ArchiveFileStreamTest.Test();
            //StorageSystem.File.TransactionalEditTest.Test();
            //StorageSystem.File.FileSystemSnapshotServiceTest.Test();

            //openHistorian.Core.CompressionTest.Test();

            //openHistorian.Core.PooledMemoryStreamTest.Test();

            ArchiveTest.Test();

            //openHistorian.Core.StorageSystem.BinaryStreamTest.Test();

            //openHistorian.Core.StorageSystem.File.BenchmarkPageSizes.Test();

            //openHistorian.Core.DataWriter.TestWritingPoints.Test();

            //openHistorian.Core.StorageSystem.Generic.LeafNodeTest.Test();

            //openHistorian.Core.StorageSystem.Generic.NodeTest.Test();
            //openHistorian.Core.StorageSystem.Specialized.NodeTest.Test();

            //Unmanaged.IndexMapTest.Test();
            //openHistorian.Core.Unmanaged.UnmanagedArrayTest.Test();
            //openHistorian.Core.StorageSystem.BufferPool.UnmanagedBufferPoolTest.Test();
        }
        public unsafe static void Test2()
        {
            //byte[] data = new byte[1024 * 1024];
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("size\tmicroseconds");

            //fixed (byte* lp = data)
            //{

            //    Stopwatch sw = new Stopwatch();
            //    sw.Start();

            //    for (int bit = 1; bit < 1024*1024; bit *= 2)
            //    {
            //        for (int x = 0; x < 1000; x++)
            //        {
            //            Unmanaged.Memory.Copy(lp, lp + 1, bit);
            //        }
            //        sw.Start();
            //        for (int x = 0; x < 100; x++)
            //        {
            //            Unmanaged.Memory.Copy(lp, lp + 1, bit);
            //            Unmanaged.Memory.Copy(lp, lp + 1, bit);
            //            Unmanaged.Memory.Copy(lp, lp + 1, bit);
            //            Unmanaged.Memory.Copy(lp, lp + 1, bit);
            //            Unmanaged.Memory.Copy(lp, lp + 1, bit);
            //            Unmanaged.Memory.Copy(lp, lp + 1, bit);
            //            Unmanaged.Memory.Copy(lp, lp + 1, bit);
            //            Unmanaged.Memory.Copy(lp, lp + 1, bit);
            //            Unmanaged.Memory.Copy(lp, lp + 1, bit);
            //            Unmanaged.Memory.Copy(lp, lp + 1, bit);
            //        }
            //        sw.Stop();
            //        sb.AppendLine(bit + "\t" + sw.Elapsed.TotalMilliseconds);
            //    }

            //}

            //Clipboard.SetText(sb.ToString());
            //MessageBox.Show(sb.ToString());


            //long ptr1, ptr2, ptr3, ptr4, ptr5;
            //Unmanaged.Memory data4 = Unmanaged.Memory.Allocate(200 * 1024 * 1024, true);
            //Unmanaged.Memory data5 = Unmanaged.Memory.Allocate(200 * 1024 * 1024, true);
            //byte[] data1 = new byte[200 * 1024 * 1024];

            //byte[] data3 = new byte[2000 * 1024 * 1024];


            //fixed (byte* ptr = data1) ptr1 = (long)ptr;
            //fixed (byte* ptr = data2) ptr2 = (long)ptr;
            //fixed (byte* ptr = data3) ptr3 = (long)ptr;
            //ptr4 = data4.Address.ToInt64();
            //ptr5 = data5.Address.ToInt64();

            //if (ptr1 == ptr2 | ptr3 == ptr4 | ptr5 == ptr1)
            //    return;
        }
    }
}
