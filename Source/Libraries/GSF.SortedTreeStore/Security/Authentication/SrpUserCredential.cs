//******************************************************************************************************
//  SrpUserCredential.cs - Gbtc
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
//  7/27/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math;

namespace GSF.Security.Authentication
{

    /// <summary>
    /// An individual server side user credential
    /// </summary>
    public class SrpUserCredential
    {
        /// <summary>
        /// Session Resume Key Name
        /// </summary>
        public readonly Guid ServerKeyName = Guid.NewGuid();

        /// <summary>
        /// Session Resume HMAC key
        /// </summary>
        public readonly byte[] ServerHMACKey = SaltGenerator.Create(32);

        /// <summary>
        /// Session Resume Encryption Key
        /// </summary>
        public readonly byte[] ServerEncryptionkey = SaltGenerator.Create(32);

        /// <summary>
        /// The normalized name of the user
        /// </summary>
        public readonly string UserName;

        public readonly byte[] UsernameBytes;
        /// <summary>
        /// The salt used to compute the password bytes (x)
        /// </summary>
        public readonly byte[] Salt;
        /// <summary>
        /// The Srp server verification bytes. (Computed as g^x % N)
        /// </summary>
        public readonly byte[] Verification;
        /// <summary>
        /// The number of SHA512 iterations using PBKDF2
        /// </summary>
        public readonly int Iterations;
        /// <summary>
        /// The bit strength of the SRP algorithm.
        /// </summary>
        public readonly SrpStrength SrpStrength;
        /// <summary>
        /// <see cref="Verification"/> as a <see cref="BigInteger"/>.
        /// </summary>
        public readonly BigInteger VerificationInteger;

        /// <summary>
        /// Creates user credentials
        /// </summary>
        /// <param name="username"></param>
        /// <param name="salt"></param>
        /// <param name="verification"></param>
        /// <param name="iterations"></param>
        /// <param name="srpStrength"></param>
        public SrpUserCredential(string username, byte[] verification, byte[] salt, int iterations, SrpStrength srpStrength)
        {
            UserName = username;
            UsernameBytes = Encoding.UTF8.GetBytes(username);
            Salt = salt;
            Verification = verification;
            Iterations = iterations;
            SrpStrength = srpStrength;
            VerificationInteger = new BigInteger(1, verification);
        }

        /// <summary>
        /// Creates a user credential from the provided data.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="strength"></param>
        /// <param name="saltSize"></param>
        /// <param name="iterations"></param>
        /// <returns></returns>
        public SrpUserCredential(string username, string password, SrpStrength strength = SrpStrength.Bits1024, int saltSize = 32, int iterations = 4000)
        {
            username = username.Normalize(NormalizationForm.FormKC);
            password = password.Normalize(NormalizationForm.FormKC);
            UsernameBytes = Encoding.UTF8.GetBytes(username);

            SrpConstants constants = SrpConstants.Lookup(strength);
            BigInteger N = constants.N;
            BigInteger g = constants.g;
            byte[] s = SaltGenerator.Create(saltSize);
            byte[] hashPassword = PBKDF2.ComputeSaltedPassword(HMACMethod.SHA512, Encoding.UTF8.GetBytes(password), s, iterations, 64);

            Sha512Digest hash = new Sha512Digest();
            byte[] output = new byte[hash.GetDigestSize()];
            hash.BlockUpdate(UsernameBytes, 0, UsernameBytes.Length);
            hash.Update((byte)':');
            hash.BlockUpdate(hashPassword, 0, hashPassword.Length);
            hash.DoFinal(output, 0);
            hash.BlockUpdate(s, 0, s.Length);
            hash.BlockUpdate(output, 0, output.Length);
            hash.DoFinal(output, 0);
            BigInteger x = new BigInteger(1, output).Mod(N);
            BigInteger v = g.ModPow(x, N);

            UserName = username;
            Salt = s;
            Verification = v.ToByteArray();
            Iterations = iterations;
            SrpStrength = strength;
            VerificationInteger = new BigInteger(1, Verification);
        }



        public void Save()
        {

        }

        public void Load()
        {

        }

    }
}


