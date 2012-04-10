using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace openHistorian.V2.Unmanaged
{
    unsafe static class UnmanagedArrayTest
    {
        public static void Test()
        {

            //const int bufferSize = 1024 * 1024 * 1024;
            //const int bufferMask = bufferSize - 1;

            ////UnmanagedBufferPool.Initialize(64 * 1024, 2 * 1024 * 1024, 1024 * 1024 * 1024, true);

            //Stopwatch sw = new Stopwatch();

            //int* lp = (int*)BufferPool.GetPageAddress(0).ToPointer();

            //sw.Start();
            ////byte[] array = new byte[bufferSize];
            //for (int x = 0; x < 1024 * 1024 * 1024 / 4; x++)
            //{
            //    lp[x] = (byte)x;
            //}
            //for (int x = 0; x < 1024 * 1024 * 1024 / 4; x++)
            //{
            //    if ((byte)x != lp[x])
            //        throw new Exception();
            //}

            ////using (UnmanagedArray array = new UnmanagedArray(100 * 1024 * 1024))
            ////{
            ////    for (int x = 0; x < 100 * 1024 * 1024; x++)
            ////    {
            ////        array[x] = (byte)x;
            ////    }
            ////    for (int x = 0; x < 100 * 1024 * 1024; x++)
            ////    {
            ////        if ((byte)x != array[x])
            ////            throw new Exception();
            ////    }
            ////}
            //sw.Stop();

            //MessageBox.Show((bufferSize / sw.Elapsed.TotalSeconds / 1000000).ToString() + " Million calls per second");


        }
    }
}
