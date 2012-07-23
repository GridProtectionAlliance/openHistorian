//******************************************************************************************************
//  HelperFunctions.cs - Gbtc
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
    static class HelperFunctions
    {
        /// <summary>
        /// Performs the given action action and throws an exception if the action
        /// does not error. This is useful for debugging code and testing for exceptions.
        /// </summary>
        /// <param name="errorFunction">the action to perform</param>
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

        /// <summary>
        /// Determines if a number is a power of 2 and outputs some useful values;
        /// </summary>
        /// <param name="value"></param>
        /// <param name="shiftBits"></param>
        /// <param name="bitMask"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(uint value, out int shiftBits, out uint bitMask)
        {
            bitMask = value - 1;
            shiftBits = CountBits(bitMask);
            return IsPowerOfTwo(value);
        }
        /// <summary>
        /// Counts the number of bits that are set
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CountBits(uint value)
        {
            uint count;
            for (count = 0; value > 0; value >>= 1)
            {
                count += value & 1;
            }
            return (int)count;
        }

        /// <summary>
        /// Determines if the number is a power of 2.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(uint value)
        {
            return value != 0 && ((value & (value - 1)) == 0);
        }

        /// <summary>
        /// Determines if a number is a power of 2 and outputs some useful values;
        /// </summary>
        /// <param name="value"></param>
        /// <param name="shiftBits"></param>
        /// <param name="bitMask"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(ulong value, out int shiftBits, out ulong bitMask)
        {
            bitMask = value - 1;
            shiftBits = CountBits(bitMask);
            return IsPowerOfTwo(value);
        }
        /// <summary>
        /// Counts the number of bits that are set
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CountBits(ulong value)
        {
            ulong count;
            for (count = 0; value > 0; value >>= 1)
            {
                count += value & 1;
            }
            return (int)count;
        }
        /// <summary>
        /// Determines if the number is a power of 2.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(ulong value)
        {
            return value != 0 && ((value & (value - 1)) == 0);
        }
        /// <summary>
        /// Rounds a number up to the nearest power of 2.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
