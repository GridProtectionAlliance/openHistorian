using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using openHistorian.V2.Collections.KeyValue;
using openHistorian.V2.IO.Unmanaged;
using openHistorian.V2.Unmanaged;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using openHistorian.V2.UnmanagedMemory;

namespace openHistorian.V2.IO.Unmanaged.Test
{

    /// <summary>
    ///This is a test class for BufferedFileStreamTest and is intended
    ///to contain all BufferedFileStreamTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BufferedFileStreamExtensiveTest
    {

        /// <summary>
        ///A test for BufferedFileStream Constructor
        ///</summary>
        [TestMethod()]
        public void BufferedFileStreamConstructorTest()
        {
            TestSize(1000*1000); // 1 million elements
        }


        public void TestSize(int elementCount)
        {
            Random r = new Random();
            int seed = r.Next();
            seed = 123948123;
            r = new Random(seed);
            StringBuilder sb = new StringBuilder();
            int currentElement=0;
            sb.AppendLine("InsertCount\tMode\tDesired Release");
            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
            try
            {

                Stopwatch SW = new Stopwatch();
                SW.Start();

                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    using (BufferPool pool = new BufferPool(65536))
                    {
                        pool.SetMaximumBufferSize(10 * 1024 * 1024);//10MB
                        pool.RequestCollection += (sender, e) => CollectionRaised(currentElement, sb, e, (BufferPool)sender);

                        using (BufferedFileStream bfs = new BufferedFileStream(fs, pool, 4096))
                        {
                            BinaryStream bs = new BinaryStream(bfs);
                            SortedTree256 tree = new SortedTree256(bs, 4096);

                            for (int x = 0; x < elementCount; x++)
                            {
                                currentElement = x;
                                ulong key1 = (ulong)r.Next();
                                ulong value1 = (ulong)r.Next();
                                ulong value2 = (ulong)r.Next();
                                tree.Add(key1, (ulong)x, value1, value2);
                            }

                            bfs.Flush();
                            
                        }
                    }
                }
                SW.Stop();
                sb.AppendLine(SW.Elapsed.TotalSeconds.ToString() + '\t' + BufferedFileStream.BytesRead / 1024.0 / 1024.0 + '\t' + BufferedFileStream.BytesWritten / 1024.0 / 1024.0);
                Clipboard.SetText(sb.ToString());
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        void CollectionRaised(int x, StringBuilder sb, CollectionEventArgs e, BufferPool sender)
        {
            sb.AppendLine(x.ToString() + '\t' + e.CollectionMode.ToString() + '\t' + e.DesiredPageReleaseCount.ToString());
        }


    }
}
