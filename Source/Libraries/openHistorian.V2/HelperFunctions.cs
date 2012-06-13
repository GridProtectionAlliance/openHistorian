using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace openHistorian.V2
{
    static class HelperFunctions
    {
        
        public static void ExpectError(Action errorFunction)
        {
            bool success;
            try
            {
                errorFunction.Invoke();
                success = true;
            }
            catch
            {
                success = false;
            }
            if (success)
                throw new Exception("This procedure should have thrown an error.");

        }

        public static bool IsPowerOfTwo(uint value, out int shiftBits, out uint bitMask)
        {
            bitMask = value - 1;
            shiftBits = CountBits(bitMask);
            return IsPowerOfTwo(value);
        }

        public static int CountBits(uint value)
        {
            uint count;
            for (count = 0; value > 0; value >>= 1)
            {
                count += value & 1;
            }
            return (int)count;
        }

        public static bool IsPowerOfTwo(uint value)
        {
            return value != 0 && ((value & (value - 1)) == 0);
        }

        public static bool IsPowerOfTwo(ulong value, out int shiftBits, out ulong bitMask)
        {
            bitMask = value - 1;
            shiftBits = CountBits(bitMask);
            return IsPowerOfTwo(value);
        }

        public static int CountBits(ulong value)
        {
            ulong count;
            for (count = 0; value > 0; value >>= 1)
            {
                count += value & 1;
            }
            return (int)count;
        }

        public static bool IsPowerOfTwo(ulong value)
        {
            return value != 0 && ((value & (value - 1)) == 0);
        }

        public static long RoundUpToNearestPowerOfTwo(long value)
        {
            long result = 1;
            while (result <= value)
            {
                result <<= 1;
            }
            return result;
        }


        /// <summary>
        /// Returns the bit position of the first 0 bit.
        /// Returns 32 if the value is -1 or all bits are set.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// Method based on a method found at: http://graphics.stanford.edu/~seander/bithacks.htm
        /// Subtitle: Count the consecutive zero bits (trailing) on the right by binary search 
        /// </remarks>
        public static int FindFirstClearedBit(int value)
        {
            int position = 0;
            if ((value & 0xffff) == 0xffff)
            {
                value >>= 16;
                position += 16;
            }
            if ((value & 0xff) == 0xff)
            {
                value >>= 8;
                position += 8;
            }
            if ((value & 0xf) == 0xf)
            {
                value >>= 4;
                position += 4;
            }
            if ((value & 0x3) == 0x3)
            {
                value >>= 2;
                position += 2;
            }
            position = position + (value & 0x1);
            return position;
        }

       

    }
}
