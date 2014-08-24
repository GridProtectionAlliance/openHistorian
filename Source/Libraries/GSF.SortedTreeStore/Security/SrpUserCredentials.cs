//******************************************************************************************************
//  SrpUserCredentials.cs - Gbtc
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
//  7/27/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math;

namespace GSF.Security
{
    /// <summary>
    /// Provides simple password based authentication that uses Secure Remote Password.
    /// </summary>
    /// <remarks>
    /// It is safe to store the user's credential on the server. This is a zero knowledge 
    /// password proof, meaning if this database is compromised, a brute force attack
    /// is the only way to reveal the password.
    /// </remarks>
    public class SrpUserCredentials
    {
        static UTF8Encoding s_utf8 = new UTF8Encoding(true);

        private Dictionary<string, UserCredentials> m_users = new Dictionary<string, UserCredentials>();

        /// <summary>
        /// An individual server side user credential
        /// </summary>
        public class UserCredentials
        {
            /// <summary>
            /// The normalized name of the user
            /// </summary>
            public readonly string UserName;
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
            internal UserCredentials(string username, byte[] salt, byte[] verification, int iterations, SrpStrength srpStrength)
            {
                UserName = username;
                Salt = salt;
                Verification = verification;
                Iterations = iterations;
                SrpStrength = srpStrength;
                VerificationInteger = new BigInteger(1, verification);
            }

            public void Save()
            {

            }

            public void Load()
            {

            }
        }

        public UserCredentials Lookup(string username)
        {
            lock (m_users)
            {
                return m_users[username];
            }
        }

        /// <summary>
        /// Adds the specified user to the credentials database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="strength"></param>
        /// <param name="saltSize"></param>
        /// <param name="iterations"></param>
        public void AddUser(string username, string password, SrpStrength strength = SrpStrength.Bits1024, int saltSize = 32, int iterations = 4000)
        {
            var user = Create(username, password, strength, saltSize, iterations);
            lock (m_users)
            {
                m_users.Add(username, user);
            }
        }

        /// <summary>
        /// Adds the specified user to the credential database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="verifier"></param>
        /// <param name="passwordSalt"></param>
        /// <param name="iterations"></param>
        /// <param name="strength"></param>
        public void AddUser(string username, byte[] verifier, byte[] passwordSalt, int iterations, SrpStrength strength)
        {
            var user = Create(username, passwordSalt, verifier, iterations, strength);
            lock (m_users)
            {
                m_users.Add(username, user);
            }
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
        public UserCredentials Create(string username, string password, SrpStrength strength = SrpStrength.Bits1024, int saltSize = 32, int iterations = 4000)
        {
            username = username.Normalize(NormalizationForm.FormKC);
            password = password.Normalize(NormalizationForm.FormKC);
            byte[] identity = s_utf8.GetBytes(username);

            var constants = SrpConstants.Lookup(strength);
            BigInteger N = constants.N;
            BigInteger g = constants.g;
            byte[] s = SaltGenerator.Create(saltSize);
            byte[] hashPassword = PBKDF2.ComputeSaltedPassword(HMACMethod.SHA512, s_utf8.GetBytes(password), s, iterations, 64);

            Sha512Digest hash = new Sha512Digest();
            byte[] output = new byte[hash.GetDigestSize()];
            hash.BlockUpdate(identity, 0, identity.Length);
            hash.Update((byte)':');
            hash.BlockUpdate(hashPassword, 0, hashPassword.Length);
            hash.DoFinal(output, 0);
            hash.BlockUpdate(s, 0, s.Length);
            hash.BlockUpdate(output, 0, output.Length);
            hash.DoFinal(output, 0);
            BigInteger x = new BigInteger(1, output).Mod(N);
            BigInteger v = g.ModPow(x, N);

            return Create(username, v.ToByteArray(), s, iterations, strength);
        }

        /// <summary>
        /// Creates a user credential from the provided data.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="verifier"></param>
        /// <param name="passwordSalt"></param>
        /// <param name="iterations"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        public UserCredentials Create(string username, byte[] verifier, byte[] passwordSalt, int iterations, SrpStrength strength)
        {
            return new UserCredentials(username, passwordSalt, verifier, iterations, strength);
        }

    }
}


