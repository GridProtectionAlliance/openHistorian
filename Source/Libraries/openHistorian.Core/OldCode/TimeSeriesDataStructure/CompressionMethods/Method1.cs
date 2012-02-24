using System;
using System.Collections;
using System.Collections.Generic;
using Historian;


namespace openHistorian.Core.TimeSeriesDataStructure.CompressionMethods
{
    public class Method1
    {
        public static int Time1 = 0;
        public static int Time2 = 0;
        public static int Time3 = 0;
        public static int Time4 = 0;
        public static int Time5 = 0;
        public static int Item1()
        {
            return 1;
        }

        public static int Compress(List<Points> data)
        {
            double Size = 0;
            //Size += CompressTimeStamp(data);
            Size += CompressValue(data);
            return (int)Size;
        }

        private static double CompressTimeStamp(List<Points> data)
        {
            return CompressTimeStampDifference(data);
        }

        private static double CompressTimeStampDifference(List<Points> data)
        {
            double Size = 0;

            SortedList<long, int> TimeDiff = new SortedList<long, int>();
            Points Prev = data[0];
            for (int x = 1; x <= data.Count - 1; x++)
            {
                Points Cur = data[x];
                long Diff = Cur.Time.Ticks - Prev.Time.Ticks;
                if (Diff == 0 | Diff < 0)
                    Diff = Diff;
                if (TimeDiff.ContainsKey(Diff))
                {
                    TimeDiff[Diff] += 1;
                }
                else
                {
                    TimeDiff.Add(Diff, 1);
                }
                Prev = Cur;
            }

            if (TimeDiff.Count == 2)
            {
                //Size += 1 * data.Count / 8
                Time1 += 1;
            }
            else if (TimeDiff.Count < 4)
            {
                Size += 2 * data.Count / 8;
                Time2 += 1;
            }
            else if (TimeDiff.Count < 8)
            {
                Size += 3 * data.Count / 8;
                Time3 += 1;
            }
            else
            {
                Size += 32 * data.Count / 8;
                Time4 += 1;
            }
            return Size;
        }

        private static double CompressValue(List<Points> data)
        {
            double RV=double.MaxValue;

            RV = Math.Min(RV, CompressValueXOR(data));
            RV = Math.Min(RV, CompressValueDifferenceXOR(data));
            RV = Math.Min(RV, CompressValuePrevXOR(data, 1));
            RV = Math.Min(RV, CompressValuePrevXOR(data, 2));
            RV = Math.Min(RV, CompressValuePrevXOR(data, 3));
            RV = Math.Min(RV, CompressValuePrevXOR(data, 4));
            RV = Math.Min(RV, CompressValuePrevXOR(data, 5));
            return RV;
        }

        private unsafe static double CompressValueXOR(List<Points> data)
        {
            int Size0 = 0;
            int Size1 = 0;
            int Size2 = 0;
            int Size3 = 0;
            int Size4 = 0;
           
            Points Prev = data[0];

            for (int x = 1; x <= data.Count - 1; x++)
            {
                Points Cur = data[x];

                int X = *(int*)&Cur.Value ^ *(int*)&Prev.Value;
          
                if ((X & 0xffffff) != X)
                    Size4++;
                else if ((X & 0xffff) != X)
                    Size3++;
                else if ((X & 0xff) != X)
                    Size2++;
                else if (X != 0)
                    Size1++;
                else
                    Size0++;

                Prev = Cur;
            }

            float type1size = 0;


            int Count = 0;
            if (Size0 > 0)
                Count++;
            if (Size1 > 0)
                Count++;
            if (Size2 > 0)
                Count++;
            if (Size3 > 0)
                Count++;
            if (Size4 > 0)
                Count++;

            if (Count == 1)
            {
                type1size += Size1;
                type1size += Size2 * 2;
                type1size += Size3 * 3;
                type1size += Size4 * 4;
            }
            if (Count == 2)
            {
                type1size += 1 * (Size0 + Size1 + Size2 + Size3 + Size4) / 8;
                type1size += Size1;
                type1size += Size2 * 2;
                type1size += Size3 * 3;
                type1size += Size4 * 4;
            }
            if (Count == 3)
            {
                type1size += 2 * (Size0 + Size1 + Size2 + Size3 + Size4) / 8;
                type1size += Size1;
                type1size += Size2 * 2;
                type1size += Size3 * 3;
                type1size += Size4 * 4;
            }
            if (Count == 4)
            {
                type1size += 2 * (Size0 + Size1 + Size2 + Size3 + Size4) / 8;
                type1size += Size1;
                type1size += Size2 * 2;
                type1size += Size3 * 3;
                type1size += Size4 * 4;
            }
            if (Count == 5)
            {
                type1size += 3 * (Size0 + Size1 + Size2 + Size3 + Size4) / 8;
                type1size += Size1;
                type1size += Size2 * 2;
                type1size += Size3 * 3;
                type1size += Size4 * 4;
            }
            return type1size;
        }

        private class SortValues : IComparer<Points>
        {
            public int Compare(Points x, Points y)
            {
                return x.Value.CompareTo(y.Value);
            }
        }
   
        private unsafe static double CompressValuePrevXOR(List<Points> data, float LookBack)
        {
            float type1size = 0;
            int BackDist = (int) Math.Pow(2, LookBack);

            for (int x = 1; x <= data.Count - 1; x++)
            {
                Points Cur = data[x];

                int Closest0 = -1;
                int Closest1 = -1;
                int Closest2 = -1;
                int Closest3 = -1;

                for (int xx = x - 1; xx >= 0 && xx >= x - BackDist; xx += -1)
                {
                    Points Prev = data[xx];
                    int X = *(int*)&Cur.Value ^ *(int*)&Prev.Value;

                    if ((X & 0xffffff) != X)
                        ;
                    else if ((X & 0xffff) != X)
                        Closest3 = Math.Max(Closest3, xx);
                    else if ((X & 0xff) != X)
                        Closest2 = Math.Max(Closest2, xx);
                    else if (X != 0)
                        Closest1 = Math.Max(Closest1, xx);
                    else
                        Closest0 = Math.Max(Closest0, xx);
                }

                if (Closest0 != -1)
                {
                    type1size += LookBack / 8;
                }
                else if (Closest1 != -1)
                {
                    type1size += LookBack / 8 + 1;
                }
                else if (Closest2 != -1)
                {
                    type1size += LookBack / 8 + 2;
                }
                else if (Closest3 != -1)
                {
                    type1size += LookBack / 8 + 3;
                }
                else
                {
                    type1size += LookBack / 8 + 4;
                }
            }
            return type1size;
        }
        
        private unsafe static double CompressValueDifferenceXOR(List<Points> data)
        {
            //data = new List<Points>(data);
            //data.Sort(new SortValues());
            //Difference first, then XOR (should cover angle measurements

            int Size0 = 0;
            int Size1 = 0;
            int Size2 = 0;
            int Size3 = 0;
            int Size4 = 0;

            Points Prev = data[0];
            float Difference = 0;
            float tmp;
            for (int x = 1; x <= data.Count - 1; x++)
            {
                Points Cur = data[x];

                tmp = Prev.Value + Difference;
                int X = *(int*)&Cur.Value ^ *(int*)&tmp;

                if ((X & 0xffffff) != X)
                    Size4++;
                else if ((X & 0xffff) != X)
                    Size3++;
                else if ((X & 0xff) != X)
                    Size2++;
                else if (X != 0)
                    Size1++;
                else
                    Size0++;

                Difference = Cur.Value - Prev.Value;
                Prev = Cur;
            }

            float type1size = 0;

            int Count = 0;
            if (Size0 > 0)
                Count++;
            if (Size1 > 0)
                Count++;
            if (Size2 > 0)
                Count++;
            if (Size3 > 0)
                Count++;
            if (Size4 > 0)
                Count++;

            type1size += Size1;
            type1size += Size2 * 2;
            type1size += Size3 * 3;
            type1size += Size4 * 4;

            if (Count == 1)
            {

            }
            if (Count == 2)
            {
                type1size += 1 * (Size0 + Size1 + Size2 + Size3 + Size4) / 8;
            }
            if (Count == 3)
            {
                type1size += 2 * (Size0 + Size1 + Size2 + Size3 + Size4) / 8;
            }
            if (Count == 4)
            {
                type1size += 2 * (Size0 + Size1 + Size2 + Size3 + Size4) / 8;
            }
            if (Count == 5)
            {
                type1size += 3 * (Size0 + Size1 + Size2 + Size3 + Size4) / 8;
            }
            return type1size;

        }

       
        //private unsafe static int FindChange(float val1, float val2)
        //{
        //    int X = *(int*)&val1 ^ *(int*)&val2;
        //    if (val1 < 0)
        //    {
        //        float Diff = val1 - val2;
        //        val1 = val1;
        //    }

        //    if ((X & 0xffffff) != X)
        //        return 4;
        //    if ((X & 0xffff) != X)
        //        return 3;
        //    if ((X & 0xff) != X)
        //        return 2;
        //    if (X != X)
        //        return 1;
        //    return 0;
        //}

    }
}