//******************************************************************************************************
//  IDigestExtensions.cs - Gbtc
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
//  8/26/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using Org.BouncyCastle.Crypto;

namespace GSF.Security
{
    /// <summary>
    /// Helper extensions for <see cref="IDigest"/> types.
    /// </summary>
    public static class IDigestExtensions
    {
        /// <summary>
        /// Computes the hash of the supplied words.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="word1"></param>
        /// <param name="word2"></param>
        /// <param name="word3"></param>
        /// <param name="word4"></param>
        /// <returns></returns>
        public static byte[] ComputeHash(this IDigest hash, byte[] word1, byte[] word2, byte[] word3, byte[] word4)
        {
            hash.BlockUpdate(word1, 0, word1.Length);
            hash.BlockUpdate(word2, 0, word2.Length);
            hash.BlockUpdate(word3, 0, word3.Length);
            hash.BlockUpdate(word4, 0, word4.Length);
            byte[] rv = new byte[hash.GetDigestSize()];
            hash.DoFinal(rv, 0);
            return rv;
        }

        /// <summary>
        /// Computes the hash of the supplied words.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="word1"></param>
        /// <param name="word2"></param>
        /// <param name="word3"></param>
        /// <returns></returns>
        public static byte[] ComputeHash(this IDigest hash, byte[] word1, byte[] word2, byte[] word3)
        {
            hash.BlockUpdate(word1, 0, word1.Length);
            hash.BlockUpdate(word2, 0, word2.Length);
            hash.BlockUpdate(word3, 0, word3.Length);
            byte[] rv = new byte[hash.GetDigestSize()];
            hash.DoFinal(rv, 0);
            return rv;
        }

        /// <summary>
        /// Computes the hash of the supplied words.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="word1"></param>
        /// <param name="word2"></param>
        /// <returns></returns>
        public static byte[] ComputeHash(this IDigest hash, byte[] word1, byte[] word2)
        {
            hash.BlockUpdate(word1, 0, word1.Length);
            hash.BlockUpdate(word2, 0, word2.Length);
            byte[] rv = new byte[hash.GetDigestSize()];
            hash.DoFinal(rv, 0);
            return rv;
        }
    }
}
