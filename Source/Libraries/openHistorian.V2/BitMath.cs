//******************************************************************************************************
//  BitMath.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  6/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;

namespace openHistorian.V2
{
    /// <summary>
    /// Contains some random and useful functions.
    /// </summary>
    static class BitMath
    {
        #region [ Is Power Of Two ]

        /// <summary>
        /// Determines if the number is a power of 2.
        /// </summary>
        /// <param name="value">The value to check power of two properties</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">If <see cref="value"/> is less than zero</exception>
        public static bool IsPowerOfTwo(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("value", "Must be greater than or equal to zero");
            return IsPowerOfTwo((uint)value);
        }

        /// <summary>
        /// Determines if the number is a power of 2.
        /// </summary>
        /// <param name="value">The value to check power of two properties</param>
        /// <exception cref="ArgumentOutOfRangeException">If <see cref="value"/> is less than zero</exception>
        public static bool IsPowerOfTwo(long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("value", "Must be greater than or equal to zero");
            return IsPowerOfTwo((ulong)value);
        }

        /// <summary>
        /// Determines if the number is a power of 2.
        /// </summary>
        /// <param name="value">The value to check power of two properties</param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(uint value)
        {
            return value != 0 && ((value & (value - 1)) == 0);
        }

        /// <summary>
        /// Determines if the number is a power of 2.
        /// </summary>
        /// <param name="value">The value to check power of two properties</param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(ulong value)
        {
            return value != 0 && ((value & (value - 1)) == 0);
        }

        #endregion
   
        #region [ Count Bits ]

        /// <summary>
        /// Counts the number of bits that are set
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CountBitsSet(uint value)
        {
            uint count;
            for (count = 0; value > 0; value >>= 1)
            {
                count += value & 1;
            }
            return (int)count;
        }

        /// <summary>
        /// Counts the number of bits that are set
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CountBitsSet(ulong value)
        {
            ulong count;
            for (count = 0; value > 0; value >>= 1)
            {
                count += value & 1;
            }
            return (int)count;
        }

        /// <summary>
        /// Counts the number of bits that are not set
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CountBitsCleared(uint value)
        {
            return CountBitsSet(~value);
        }

        /// <summary>
        /// Counts the number of bits that are not set
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CountBitsCleared(ulong value)
        {
            return CountBitsSet(~value);
        }

        #endregion

        #region [ Round To Power Of Two ]

        /// <summary>
        /// Rounds a number up to the nearest power of 2.
        /// If the value is a power of two, the same value is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ulong RoundUpToNearestPowerOfTwo(ulong value)
        {
            ulong result = 1;
            while (result < value)
            {
                result <<= 1;
            }
            return result;
        }
        /// <summary>
        /// Rounds a number up to the nearest power of 2.
        /// If the value is a power of two, the same value is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint RoundUpToNearestPowerOfTwo(uint value)
        {
            uint result = 1;
            while (result < value)
            {
                result <<= 1;
            }
            return result;
        }

        /// <summary>
        /// Rounds a number down to the nearest power of 2.
        /// If the value is a power of two, the same value is returned.
        /// If value is zero, zero is returned (which is not a valid power of two)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ulong RoundDownToNearestPowerOfTwo(ulong value)
        {
            ulong result = ulong.MaxValue;
            while (result > value)
            {
                result >>= 1;
            }
            return result;
        }
        /// <summary>
        /// Rounds a number down to the nearest power of 2.
        /// If the value is a power of two, the same value is returned.
        /// If value is zero, zero is returned (which is not a valid power of two)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint RoundDownToNearestPowerOfTwo(uint value)
        {
            uint result = uint.MaxValue;
            while (result > value)
            {
                result >>= 1;
            }
            return result;
        }

        #endregion

        /// <summary>
        /// Creates a bit mask for a number with the given number of bits.
        /// </summary>
        /// <param name="bitCount"></param>
        /// <returns></returns>
        public static ulong CreateBitMask(int bitCount)
        {
            return ulong.MaxValue >> (64 - bitCount);
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
