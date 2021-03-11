//******************************************************************************************************
//  ScramClient.cs - Gbtc
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

using System.IO;
using System.Text;
using GSF.IO;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace GSF.Security.Authentication
{
    /// <summary>
    /// Provides simple password based authentication that uses Secure Remote Password.
    /// </summary>
    public class ScramClient
    {
        private readonly NonceGenerator m_nonce = new NonceGenerator(16);
        private readonly byte[] m_usernameBytes;
        private readonly byte[] m_passwordBytes;
        private byte[] m_salt;
        private int m_iterations;
        private byte[] m_saltedPassword;
        private byte[] m_serverKey;
        private byte[] m_clientKey;
        private byte[] m_storedKey;
        private HMac m_clientSignature;
        private HMac m_serverSignature;
        private HashMethod m_hashMethod;

        /// <summary>
        /// Creates a new set of client credentials.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public ScramClient(string username, string password)
        {
            m_usernameBytes = Scram.Utf8.GetBytes(username.Normalize(NormalizationForm.FormKC));
            m_passwordBytes = Scram.Utf8.GetBytes(password.Normalize(NormalizationForm.FormKC));
        }

        /// <summary>
        /// Sets the server parameters and regenerates the salted password if 
        /// the salt values have changed.
        /// </summary>
        /// <param name="hashMethod">the hashing method</param>
        /// <param name="salt">the salt for the user credentials.</param>
        /// <param name="iterations">the number of iterations.</param>
        private void SetServerValues(HashMethod hashMethod, byte[] salt, int iterations)
        {
            bool hasPasswordDataChanged = false;
            bool hasHashMethodChanged = false;

            if (m_salt is null || !salt.SecureEquals(m_salt))
            {
                hasPasswordDataChanged = true;
                m_salt = salt;
            }
            if (iterations != m_iterations)
            {
                hasPasswordDataChanged = true;
                m_iterations = iterations;
            }
            if (m_hashMethod != hashMethod)
            {
                m_hashMethod = hashMethod;
                hasHashMethodChanged = true;
            }

            if (hasPasswordDataChanged)
            {
                m_saltedPassword = Scram.GenerateSaltedPassword(m_passwordBytes, m_salt, m_iterations);
            }
            if (hasPasswordDataChanged || hasHashMethodChanged)
            {
                m_serverKey = Scram.ComputeServerKey(m_hashMethod, m_saltedPassword);
                m_clientKey = Scram.ComputeClientKey(m_hashMethod, m_saltedPassword);
                m_storedKey = Scram.ComputeStoredKey(m_hashMethod, m_clientKey);
                m_clientSignature = new HMac(Scram.CreateDigest(m_hashMethod));
                m_clientSignature.Init(new KeyParameter(m_storedKey));

                m_serverSignature = new HMac(Scram.CreateDigest(m_hashMethod));
                m_serverSignature.Init(new KeyParameter(m_serverKey));
            }
        }

        private byte[] ComputeClientSignature(byte[] authMessage)
        {
            byte[] result = new byte[m_clientSignature.GetMacSize()];
            lock (m_clientSignature)
            {
                m_clientSignature.BlockUpdate(authMessage, 0, authMessage.Length);
                m_clientSignature.DoFinal(result, 0);
            }
            return result;
        }

        private byte[] ComputeServerSignature(byte[] authMessage)
        {
            byte[] result = new byte[m_serverSignature.GetMacSize()];
            lock (m_serverSignature)
            {
                m_serverSignature.BlockUpdate(authMessage, 0, authMessage.Length);
                m_serverSignature.DoFinal(result, 0);
            }
            return result;
        }

        public bool AuthenticateAsClient(Stream stream, byte[] additionalChallenge = null)
        {
            if (additionalChallenge is null)
                additionalChallenge = new byte[] { };

            byte[] clientNonce = m_nonce.Next();
            stream.WriteWithLength(m_usernameBytes);
            stream.WriteWithLength(clientNonce);
            stream.Flush();

            HashMethod hashMethod = (HashMethod)stream.ReadByte();
            byte[] serverNonce = stream.ReadBytes();
            byte[] salt = stream.ReadBytes();
            int iterations = stream.ReadInt32();

            SetServerValues(hashMethod, salt, iterations);

            byte[] authMessage = Scram.ComputeAuthMessage(serverNonce, clientNonce, salt, m_usernameBytes, iterations, additionalChallenge);
            byte[] clientSignature = ComputeClientSignature(authMessage);
            byte[] clientProof = Scram.XOR(m_clientKey, clientSignature);
            stream.WriteWithLength(clientProof);
            stream.Flush();

            byte[] serverSignature = ComputeServerSignature(authMessage);
            byte[] serverSignatureVerify = stream.ReadBytes();
            return serverSignature.SecureEquals(serverSignatureVerify);
        }
    }
}


