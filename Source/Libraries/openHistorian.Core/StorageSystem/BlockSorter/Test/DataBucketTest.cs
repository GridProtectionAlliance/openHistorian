using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Historian.StorageSystem.BlockSorter
{
    class DataBucketTest
    {
        public static void Test()
        {
            TreeHeader tree = new TreeHeader(new BinaryStream(new PooledMemoryStream()), 4096);
            byte[] data1 = new byte[] { 1 };
            byte[] data2 = new byte[] { 1, 2 };
            byte[] data3 = new byte[] { 1, 2, 3 };
            byte[] data4 = new byte[] { 1, 2, 3, 4 };
            byte[] data5 = new byte[] { 1, 2, 3, 4, 5 };
            byte[] data6 = new byte[] { 1, 2, 3, 4, 5, 6 };

            if (BPlusTreeDataBucket.Write(tree, data1) != 4096 * 2 + 0) throw new Exception();
            if (BPlusTreeDataBucket.Write(tree, data2) != 4096 * 2 + 2) throw new Exception();
            if (BPlusTreeDataBucket.Write(tree, data3) != 4096 * 2 + 5) throw new Exception();
            if (BPlusTreeDataBucket.Write(tree, data4) != 4096 * 2 + 9) throw new Exception();
            if (BPlusTreeDataBucket.Write(tree, data5) != 4096 * 2 + 14) throw new Exception();
            if (BPlusTreeDataBucket.Write(tree, data6) != 4096 * 2 + 20) throw new Exception();

            if (!data1.SequenceEqual(BPlusTreeDataBucket.Read(tree, 4096 * 2 + 0))) throw new Exception();
            if (!data2.SequenceEqual(BPlusTreeDataBucket.Read(tree, 4096 * 2 + 2))) throw new Exception();
            if (!data3.SequenceEqual(BPlusTreeDataBucket.Read(tree, 4096 * 2 + 5))) throw new Exception();
            if (!data4.SequenceEqual(BPlusTreeDataBucket.Read(tree, 4096 * 2 + 9))) throw new Exception();
            if (!data5.SequenceEqual(BPlusTreeDataBucket.Read(tree, 4096 * 2 + 14))) throw new Exception();
            if (!data6.SequenceEqual(BPlusTreeDataBucket.Read(tree, 4096 * 2 + 20))) throw new Exception();

            if (BPlusTreeDataBucket.Read(tree, -1) != null) throw new Exception();
        }
    }
}
