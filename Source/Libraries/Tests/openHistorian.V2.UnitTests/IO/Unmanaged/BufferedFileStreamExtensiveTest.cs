using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using openHistorian.Collections.KeyValue;
using openHistorian.IO.Unmanaged;
using System;
using System.IO;
using openHistorian.UnmanagedMemory;

namespace openHistorian.IO.Unmanaged.Test
{

    /// <summary>
    ///This is a test class for BufferedFileStreamTest and is intended
    ///to contain all BufferedFileStreamTest Unit Tests
    ///</summary>
    [TestFixture()]
    public class BufferedFileStreamExtensiveTest
    {

        /// <summary>
        ///A test for BufferedFileStream Constructor
        ///</summary>
        [Test()]
        public void BufferedFileStreamConstructorTest()
        {
            TestSize(1000 * 1000); // 1 million elements
        }

        /// <summary>
        ///A test for BufferedFileStream Constructor
        ///</summary>
        [Test()]
        public void RawTest()
        {
            TestSize2(1000 * 10); // 1 million elements
        }

        /// <summary>
        ///A test for BufferedFileStream Constructor
        ///</summary>
        [Test()]
        public void BufferedFileStreamConstructorTestRW()
        {
            TestSizeReadAndWrite(1000 * 1000); // 1 million elements
        }

        public void TestSize(int elementCount)
        {
            Random r = new Random();
            int seed = r.Next();
            seed = 123948123;
            r = new Random(seed);
            StringBuilder sb = new StringBuilder();
            int currentElement = 0;
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
                        pool.SetMaximumBufferSize(400 * 1024 * 1024);//10MB
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
                //Clipboard.SetText(sb.ToString());
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        public void TestSizeReadAndWrite(int elementCount)
        {
            Random r = new Random();
            int seed = r.Next();
            r = new Random(seed);
            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
            try
            {
                Stopwatch SW = new Stopwatch();
                SW.Start();

                using (BufferPool pool2 = new BufferPool())
                using (BinaryStream BS = new BinaryStream(pool2))
                {
                    SortedTree256 tree2 = new SortedTree256(BS, 4096);

                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    {
                        using (BufferPool pool = new BufferPool(65536))
                        {
                            pool.SetMaximumBufferSize(400 * 1024 * 1024);//10MB

                            using (BufferedFileStream bfs = new BufferedFileStream(fs, pool, 4096))
                            {
                                BinaryStream bs = new BinaryStream(bfs);
                                SortedTree256 tree = new SortedTree256(bs, 4096);

                                for (int x = 0; x < elementCount; x++)
                                {
                                    ulong key1 = (ulong)r.Next();
                                    ulong value1 = (ulong)r.Next();
                                    ulong value2 = (ulong)r.Next();
                                    tree.Add(key1, (ulong)x, value1, value2);
                                    tree2.Add(key1, (ulong)x, value1, value2);
                                }

                                bfs.Flush();
                            }
                        }
                    }

                    using (FileStream fs = new FileStream(fileName, FileMode.Open))
                    {
                        using (BufferPool pool = new BufferPool(65536))
                        {
                            pool.SetMaximumBufferSize(400 * 1024 * 1024);//10MB

                            using (BufferedFileStream bfs = new BufferedFileStream(fs, pool, 4096))
                            {
                                BinaryStream bs = new BinaryStream(bfs);
                                SortedTree256 tree = new SortedTree256(bs);

                                var item1 = tree.GetDataRange();
                                var item2 = tree2.GetDataRange();

                                ulong x1, x2, x3, x4;
                                ulong y1, y2, y3, y4;

                                bool b1 = true;
                                bool b2 = true;

                                while (b1 & b2)
                                {
                                    b1 = item1.GetNextKey(out x1, out x2, out x3, out x4);
                                    b2 = item2.GetNextKey(out y1, out y2, out y3, out y4);

                                    Assert.AreEqual(b1, b1);
                                    Assert.AreEqual(x1, y1);
                                    Assert.AreEqual(x2, y2);
                                    Assert.AreEqual(x3, y3);
                                    Assert.AreEqual(x4, y4);
                                }

                                Assert.AreEqual(b1, b1);
                            }
                        }
                    }

                }


                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    using (BufferPool pool = new BufferPool(65536))
                    {
                        pool.SetMaximumBufferSize(400 * 1024 * 1024);//10MB

                        using (BufferedFileStream bfs = new BufferedFileStream(fs, pool, 4096))
                        {
                            BinaryStream bs = new BinaryStream(bfs);
                            SortedTree256 tree = new SortedTree256(bs, 4096);

                            for (int x = 0; x < elementCount; x++)
                            {
                                ulong key1 = (ulong)r.Next();
                                ulong value1 = (ulong)r.Next();
                                ulong value2 = (ulong)r.Next();
                                tree.Add(key1, (ulong)x, value1, value2);
                            }

                            bfs.Flush();
                        }
                    }
                }
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

        public void TestSize2(int elementCount)
        {
            Random r = new Random();
            int seed = r.Next();
            seed = 123948123;
            r = new Random(seed);
            StringBuilder sb = new StringBuilder();
            int currentElement = 0;
            sb.AppendLine("InsertCount\tMode\tDesired Release");
            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
            try
            {

                Stopwatch SW = new Stopwatch();
                SW.Start();

                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {

                    BinaryStreamWrapper bs = new BinaryStreamWrapper(fs);
                    SortedTree256 tree = new SortedTree256(bs, 4096);

                    for (int x = 0; x < elementCount; x++)
                    {
                        currentElement = x;
                        ulong key1 = (ulong)r.Next();
                        ulong value1 = (ulong)r.Next();
                        ulong value2 = (ulong)r.Next();
                        tree.Add(key1, (ulong)x, value1, value2);
                    }

                }
                SW.Stop();
                sb.AppendLine(SW.Elapsed.TotalSeconds.ToString() + '\t' + BufferedFileStream.BytesRead / 1024.0 / 1024.0 + '\t' + BufferedFileStream.BytesWritten / 1024.0 / 1024.0);
                //Clipboard.SetText(sb.ToString());


            }
            finally
            {
                File.Delete(fileName);
            }
        }


    }
}
