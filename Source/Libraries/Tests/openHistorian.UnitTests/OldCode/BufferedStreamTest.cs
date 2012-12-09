//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;

//namespace openHistorian.Unmanaged
//{
//    class BufferedStreamTest
//    {
//        public unsafe static void Test()
//        {
//            //string file = Path.GetTempFileName();
//            string file = "g:\\HistorianTmpFile.tmp";
//            if (File.Exists(file))
//                File.Delete(file);
//            try
//            {
//                Random rand = new Random();
//                int seed = rand.Next();// rand.Next();
//                rand = new Random(seed);
//                byte[] data = new byte[16];
//                byte[] data2 = new byte[16];

//                using (FileStream fs = new FileStream(file, FileMode.CreateNew))
//                {
//                    BufferedStream bs = new BufferedStream(fs);

//                    int position = 0;
//                    for (int x = 0; x < 1000; x++)
//                    {
//                        rand.NextBytes(data);
//                        bs.Write(position, data, 0, data.Length);
//                        position += 16 + rand.Next(6536);
//                    }

//                    bs.Flush();
//                    bs = new BufferedStream(fs);

//                    rand = new Random(seed);
//                    position = 0;

//                    for (int x = 0; x < 1000; x++)
//                    {
//                        rand.NextBytes(data);
//                        bs.Read(position, data2, 0, 16);
//                        if (!data2.SequenceEqual<byte>(data)) throw new Exception();
//                        position += 16 + rand.Next(6536);
//                    }
//                }
//            }
//            finally
//            {
//                //File.Delete(file);
//            }

//        }
//        public unsafe static void BenchmarkTest()
//        {

//            const FileOptions FileFlagNoBuffering = (FileOptions)0x20000000;

//            //string file = Path.GetTempFileName();
//            string file = "g:\\HistorianTmpFile.tmp";
//            //if (File.Exists(file))
//            //    File.Delete(file);
//            try
//            {
//                Random rand = new Random();
//                int seed = rand.Next();// rand.Next();
//                rand = new Random(seed);
//                byte[] data = new byte[16];
//                byte[] data2 = new byte[16];
//                rand.NextBytes(data);

//                Stopwatch sw1 = new Stopwatch();
//                Stopwatch sw2 = new Stopwatch();
//                sw1.Start();

//                BufferPool.SetMinimumMemoryUsage(BufferPool.MaximumMemoryUsage);

//                //using (FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096, FileFlagNoBuffering))
//                using (FileStream fs = new FileStream(file, FileMode.OpenOrCreate))
//                {

//                    BufferedStream bs = new BufferedStream(fs);

//                    for (int x = 0; x < 1000000; x++)
//                    {
//                        bs.Write(rand.Next(2000 * 1024 * 1024), data, 0, data.Length);
//                    }

//                    bs.Flush();
//                    fs.Flush(true);
//                    WinApi.FlushFileBuffers(fs.SafeFileHandle);

//                    bs = new BufferedStream(fs);

//                    rand = new Random(seed);

//                    for (int x = 0; x < 100000; x++)
//                    {
//                        bs.Read(rand.Next(200 * 1024 * 1024), data2, 0, data2.Length);
//                    }
//                    fs.Flush(true);
//                    fs.Close();

//                }
//                sw1.Stop();

//                //File.Delete(file);

//                sw2.Start();
//                //using (FileStream fs = new FileStream(file, FileMode.CreateNew))
//                //{

//                //    for (int x = 0; x < 100000; x++)
//                //    {
//                //        fs.Position = rand.Next(2 * 1024 * 1024);
//                //        fs.Write(data, 0, data.Length);
//                //    }

//                //    fs.Flush();

//                //    rand = new Random(seed);

//                //    for (int x = 0; x < 100000; x++)
//                //    {
//                //        fs.Position = rand.Next(2 * 1024 * 1024);
//                //        fs.Read(data2, 0, data2.Length);
//                //    }
//                //}
//                sw2.Stop();

//                MessageBox.Show("Cached:" + sw1.Elapsed.TotalSeconds + " Windows Cache:" + sw2.Elapsed.TotalSeconds);
//            }
//            finally
//            {
//                //File.Delete(file);
//            }


//        }
//    }
//}
