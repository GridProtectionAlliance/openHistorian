//******************************************************************************************************
//  HMAC.cs - Gbtc
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

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace GSF.Security
{

    public static class HMAC
    {
        public static byte[] Compute(IDigest hash, byte[] key, byte[] data)
        {
            return Compute(hash, key, data, 0, data.Length);
        }

        public static byte[] Compute(IDigest hash, byte[] key, byte[] data, int position, int length)
        {
            data.ValidateParameters(position, length);
            HMac hmac = new HMac(hash);
            hmac.Init(new KeyParameter(key));
            byte[] result = new byte[hmac.GetMacSize()];
            hmac.BlockUpdate(data, position, length);
            hmac.DoFinal(result, 0);
            return result;
        }
    }

    public static class HMAC<T>
      where T : IDigest, new()
    {
        public static byte[] Compute(byte[] key, byte[] data)
        {
            return HMAC.Compute(new T(), key, data, 0, data.Length);
        }

        public static byte[] Compute(byte[] key, byte[] data, int position, int length)
        {
            return HMAC.Compute(new T(), key, data, position, length);
        }
    }
}
