//******************************************************************************************************
//  NonceGenerator.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
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
//  8/23/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Security.Cryptography;
using System.Threading;

namespace GSF.Security
{
    /// <summary>
    /// Used to generate Nonce values. Only use if a high speed nonce value is needed
    /// and it does not compromise the security of the protocol.
    /// </summary>
    /// <remarks>
    /// Since it is only required that a nonce does not need to repeat,
    /// A secure nonce will be generated at first call. Then the nonce will
    /// simply be incremented after this point.
    /// </remarks>
    public class NonceSequencer
    {
        private long m_nonceNumber;

        private byte[] m_startingNonce;
        /// <summary>
        /// Creates a nonce generator of the specified length.
        /// </summary>
        /// <param name="length">the size of each nonce. Must be at least 16 bytes.</param>
        public NonceSequencer(int length)
        {
            if (length < 16)
                throw new ArgumentOutOfRangeException("length", "Cannot be less than 16");
            m_nonceNumber = 0;
            m_startingNonce = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(m_startingNonce);
            }
            long time = DateTime.UtcNow.Ticks;
            BigEndian.CopyBytes(time, m_startingNonce, 0);
        }

        /// <summary>
        /// Gets the next nonce value.
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] Next()
        {
            long value = Interlocked.Increment(ref m_nonceNumber);
            byte[] rv = (byte[])m_startingNonce.Clone();
            fixed (byte* lp = &rv[rv.Length - 8])
            {
                *(long*)lp += value;
            }
            return rv;
        }

    }
}
