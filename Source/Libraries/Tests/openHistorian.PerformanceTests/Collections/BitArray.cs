using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using openHistorian.Collections;

namespace openHistorian.UnmanagedMemory.Test
{
    [TestFixture()]
    public class BitArrayTest
    {
        [Test]
        public void BitArray()
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            Stopwatch sw3 = new Stopwatch();
            Stopwatch sw4 = new Stopwatch();
            Stopwatch sw5 = new Stopwatch();
            Stopwatch sw6 = new Stopwatch();

            const int count = 20 * 1024 * 1024;

            //20 million, That's like 120GB of 64KB pages
            BitArray array = new BitArray(count, false);

            sw1.Start();
            for (int x = 0; x < count; x++)
            {
                array.SetBit(x);
            }
            sw1.Stop();

            sw2.Start();
            for (int x = 0; x < count; x++)
            {
                array.SetBit(x);
            }
            sw2.Stop();

            sw3.Start();
            for (int x = 0; x < count; x++)
            {
                array.ClearBit(x);
            }
            sw3.Stop();

            sw4.Start();
            for (int x = 0; x < count; x++)
            {
                array.ClearBit(x);
            }
            sw4.Stop();

            sw5.Start();
            for (int x = 0; x < count; x++)
            {
                array.GetBit(x);
            }
            sw5.Stop();

            //for (int x = 0; x < count -1; x++)
            //{
            //    array.SetBit(x);
            //}

            sw6.Start();
            for (int x = 0; x < count; x++)
            {
                array.SetBit(array.FindClearedBit());
            }
            sw6.Stop();

            Console.WriteLine("Set Bits: " + (count / sw1.Elapsed.TotalSeconds / 1000000).ToString("0.0 MPP"));
            Console.WriteLine("Set Bits Again: " + (count / sw2.Elapsed.TotalSeconds / 1000000).ToString("0.0 MPP"));
            Console.WriteLine("Clear Bits: " + (count / sw3.Elapsed.TotalSeconds / 1000000).ToString("0.0 MPP"));
            Console.WriteLine("Clear Bits Again: " + (count / sw4.Elapsed.TotalSeconds / 1000000).ToString("0.0 MPP"));
            Console.WriteLine("Get Bits: " + (count / sw5.Elapsed.TotalSeconds / 1000000).ToString("0.0 MPP"));
            Console.WriteLine("Find Cleared Bit (All bits cleared): " + (count / sw6.Elapsed.TotalSeconds / 1000000).ToString("0.0 MPP"));
          

        }


    }
}
