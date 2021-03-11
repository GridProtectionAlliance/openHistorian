//******************************************************************************************************
//  SecurityExtensions.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  8/23/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Runtime.CompilerServices;

namespace GSF.Security
{
    public static class SecurityExtensions
    {
        /// <summary>
        /// Does a time constant comparison of the two byte arrays. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>true if both arrays are equal</returns>
        /// <remarks>
        /// If a or b is null, function returns immediately with a false.
        /// 
        /// Certain cryptographic attacks can occur by comparing the amount of time it
        /// takes to do certain operations. Comparing two byte arrays is one example.
        /// Therefore this method should take constant time to do a comparison of two arrays.
        /// </remarks>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static bool SecureEquals(this byte[] a, byte[] b)
        {
            if (a is null || b is null)
                return false;
            int difference = a.Length ^ b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                difference |= a[i] ^ b[i];
            }
            return difference == 0;
        }

        /// <summary>
        /// Does a time constant comparison of the two byte arrays. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="bPosition">the start position of <see cref="b"/></param>
        /// <param name="bLength">the length of <see cref="b"/> to read past <see cref="bPosition"/></param>
        /// <returns>true if both arrays are equal</returns>
        /// <remarks>
        /// If a or b is null, function returns immediately with a false.
        /// 
        /// Certain cryptographic attacks can occur by comparing the amount of time it
        /// takes to do certain operations. Comparing two byte arrays is one example.
        /// Therefore this method should take constant time to do a comparison of two arrays.
        /// </remarks>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static bool SecureEquals(this byte[] a, byte[] b, int bPosition, int bLength)
        {
            b.ValidateParameters(bPosition, bLength);
            if (a is null || b is null)
                return false;
            int difference = a.Length ^ bLength;
            for (int ia = 0, ib = bPosition; ia < a.Length && ib < b.Length; ia++, ib++)
            {
                difference |= a[ia] ^ b[ib];
            }
            return difference == 0;
        }

        /// <summary>
        /// Does a time constant comparison of the two Guids. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>true if both Guids are equal</returns>
        /// <remarks>
        /// Certain cryptographic attacks can occur by comparing the amount of time it
        /// takes to do certain operations. Comparing two byte arrays is one example.
        /// Therefore this method should take constant time to do a comparison of two Guids.
        /// </remarks>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public unsafe static bool SecureEquals(this Guid a, Guid b)
        {
            int* lpa = (int*)&a;
            int* lpb = (int*)&b;
            int difference = lpa[0] ^ lpb[0];
            difference |= lpa[1] ^ lpb[1];
            difference |= lpa[2] ^ lpb[2];
            difference |= lpa[3] ^ lpb[3];
            return difference == 0;
        }

    }
}
