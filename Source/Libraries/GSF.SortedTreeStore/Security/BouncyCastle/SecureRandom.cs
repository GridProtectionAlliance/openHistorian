//using System;
//using System.Security.Cryptography;

//namespace Org.BouncyCastle.Security
//{
//    public class SecureRandom
//        : Random
//    {
//        protected static RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();
        
//        public override int Next()
//        {
//            for (; ; )
//            {
//                int i = NextInt() & int.MaxValue;

//                if (i != int.MaxValue)
//                    return i;
//            }
//        }

//        public override int Next(int maxValue)
//        {
//            if (maxValue < 2)
//            {
//                if (maxValue < 0)
//                    throw new ArgumentOutOfRangeException("maxValue < 0");

//                return 0;
//            }

//            // Test whether maxValue is a power of 2
//            if ((maxValue & -maxValue) == maxValue)
//            {
//                int val = NextInt() & int.MaxValue;
//                long lr = ((long)maxValue * (long)val) >> 31;
//                return (int)lr;
//            }

//            int bits, result;
//            do
//            {
//                bits = NextInt() & int.MaxValue;
//                result = bits % maxValue;
//            }
//            while (bits - result + (maxValue - 1) < 0); // Ignore results near overflow

//            return result;
//        }

//        public override int Next(int minValue,int maxValue)
//        {
//            if (maxValue <= minValue)
//            {
//                if (maxValue == minValue)
//                    return minValue;

//                throw new ArgumentException("maxValue cannot be less than minValue");
//            }

//            int diff = maxValue - minValue;
//            if (diff > 0)
//                return minValue + Next(diff);

//            for (; ; )
//            {
//                int i = NextInt();

//                if (i >= minValue && i < maxValue)
//                    return i;
//            }
//        }

//        public override void NextBytes(byte[] buffer)
//        {
//            Rng.GetBytes(buffer);
//        }

//        private static readonly double DoubleScale = System.Math.Pow(2.0, 64.0);

//        public override double NextDouble()
//        {
//            return Convert.ToDouble((ulong)NextLong()) / DoubleScale;
//        }

//        public virtual int NextInt()
//        {
//            byte[] intBytes = new byte[4];
//            NextBytes(intBytes);

//            int result = 0;
//            for (int i = 0; i < 4; i++)
//            {
//                result = (result << 8) + (intBytes[i] & 0xff);
//            }

//            return result;
//        }

//        public virtual long NextLong()
//        {
//            return ((long)(uint)NextInt() << 32) | (long)(uint)NextInt();
//        }
//    }
//}
