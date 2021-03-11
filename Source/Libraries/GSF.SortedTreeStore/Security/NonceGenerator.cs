//******************************************************************************************************
//  NonceGenerator.cs - Gbtc
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
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;

namespace GSF.Security
{
    /// <summary>
    /// Used to generate Nonce values. 
    /// </summary>
    public unsafe class NonceGenerator
    {
        /// <summary>
        /// A sequence number to ensure that duplicates are never created
        /// </summary>
        private long m_nonceNumber;
        /// <summary>
        /// The secure random number that serves as the basis for this nonce
        /// </summary>
        private readonly byte[] m_startingNonce;
        /// <summary>
        /// Creates a nonce generator of the specified length.
        /// </summary>
        /// <param name="length">the size of each nonce. Must be at least 16 bytes.</param>
        public NonceGenerator(int length)
        {
            if (length < 16)
                throw new ArgumentOutOfRangeException("length", "Cannot be less than 16");
            m_nonceNumber = 0;
            m_startingNonce = new byte[length];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(m_startingNonce);
            }
        }

        /// <summary>
        /// Gets the next nonce value.
        /// </summary>
        /// <returns></returns>
        public byte[] Next()
        {
            long date = Stopwatch.GetTimestamp();
            long value = Interlocked.Increment(ref m_nonceNumber);
            byte[] rv = (byte[])m_startingNonce.Clone();
            fixed (byte* lp = rv)
            {
                *(long*)lp ^= date;
                *(long*)(lp + rv.Length - 8) ^= value;
            }
            return rv;
        }

    }
}
