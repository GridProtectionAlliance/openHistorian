using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using GSF;
using GSF.Snap.Types;
using NUnit.Framework;

namespace openHistorian.Collections
{
    [TestFixture]
    public class BinarySearchTest
    {
        public static void Shuffle<T>(IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box); while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = box[0] % n;
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Shuffle<T>(IList<T> list, int seed, int loopCount)
        {
            Random rng = new Random(seed);
            for (int l = 0; l < loopCount; l++)
            {
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    T value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
                rng = new Random(rng.Next());
            }
        }

        [Test]
        public unsafe void TestRandom()
        {
            int x = 64;
            while (x < 10000)
            {
                TestRandom(x);
                x *= 2;
            }
        }

        public unsafe void TestRandom(int count)
        {
            int loopCount = 200;

            uint[] items = new uint[count];
            uint[] lookupList = new uint[count];

            for (uint x = 0; x < items.Length; x++)
            {
                items[x] = 2 * x;
                lookupList[x] = 2 * x + 1;
            }
            Shuffle(lookupList, 3, 10);

            StepTimer.Reset();
            for (int cnt = 0; cnt < loopCount; cnt++)
            {
                //GC.Collect(0);

                //items = (uint[])items.Clone();

                //GC.WaitForPendingFinalizers();
                //System.Threading.Thread.Sleep(10);

                SnapCustomMethodsUInt32 bin = new SnapCustomMethodsUInt32();
                fixed (uint* lp = items)
                {
                    byte* lpp = (byte*)lp;
                    SnapUInt32 box = new SnapUInt32();

                    StepTimer.ITimer timer = StepTimer.Start("Lookup");
                    for (int x = 0; x < lookupList.Length; x++)
                    {
                        box.Value = lookupList[x];
                        bin.BinarySearch(lpp, box, count, 4);
                        //BoxKeyMethodsUint32.BinarySearchTest(lpp, box, count, 4);
                    }
                    timer.Stop();
                }
            }

            StringBuilder SB = new StringBuilder();
            //Console.Write(count.ToString("Tree\t0\t"));
            //SB.Append((count * 4).ToString("0\t") + (count / StepTimer.GetAverage("Lookup") / 1000000).ToString("0.000\t"));
            //SB.Append((count * 4.0 / 1024).ToString("0.###\t") + ((StepTimer.GetAverage("Lookup") / Math.Log(count, 2)) / count * 1000000000).ToString("0.00\t"));
            SB.Append((StepTimer.GetSlowest("Lookup") / Math.Log(count, 2) / count * 1000000000).ToString("0.00\t"));
            //SB.Append(((StepTimer.GetAverage("Lookup") / Math.Log(count, 2)) / count * 1000000000).ToString("0.00\t"));
            Console.WriteLine(SB.ToString());
        }
    }
}