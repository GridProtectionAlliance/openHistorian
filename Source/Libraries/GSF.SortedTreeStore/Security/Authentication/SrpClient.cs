//******************************************************************************************************
//  SrpClient.cs - Gbtc
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
using System.IO;
using GSF.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement.Srp;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math;

namespace GSF.Security.Authentication
{
    /// <summary>
    /// Provides simple password based authentication that uses Secure Remote Password.
    /// </summary>
    public class SrpClient
    {
        private readonly PBDKFCredentials m_credentials;

        //The SRP Mechanism
        private int m_srpByteLength;
        private SrpStrength m_strength;
        private SrpConstants m_param;
        private Srp6Client m_client;
        private IDigest m_hash;

        /// <summary>
        /// Session Resume Details
        /// </summary>
        private byte[] m_sessionSecret;
        private byte[] m_resumeTicket;

        /// <summary>
        /// Creates a client that will authenticate with the specified 
        /// username and password.
        /// </summary>
        /// <param name="username">the username</param>
        /// <param name="password">the password</param>
        public SrpClient(string username, string password)
        {
            if (username is null)
                throw new ArgumentNullException("username");
            if (password is null)
                throw new ArgumentNullException("username");
         

            m_credentials = new PBDKFCredentials(username, password);
            if (m_credentials.UsernameBytes.Length > 1024)
                throw new ArgumentException("Username cannot consume more than 1024 bytes encoded as UTF8", "username");
        }

        private void SetSrpStrength(SrpStrength strength)
        {
            m_strength = strength;
            m_srpByteLength = (int)strength >> 3;
            m_param = SrpConstants.Lookup(m_strength);
            m_client = new Srp6Client(m_param);
        }

        private void SetHashMethod(HashMethod hashMethod)
        {
            m_hash = hashMethod switch
            {
                HashMethod.Sha1   => new Sha1Digest(),
                HashMethod.Sha256 => new Sha256Digest(),
                HashMethod.Sha384 => new Sha384Digest(),
                HashMethod.Sha512 => new Sha512Digest(),
                _                 => throw new Exception("Unsupported Hash Method")
            };
        }

        public bool AuthenticateAsClient(Stream stream, byte[] additionalChallenge = null)
        {
            if (additionalChallenge is null)
                additionalChallenge = new byte[] { };

            stream.Write((short)m_credentials.UsernameBytes.Length);
            stream.Write(m_credentials.UsernameBytes);

            if (m_resumeTicket != null)
            {
                //resume
                stream.Write((byte)2);
                stream.Flush();

                return ResumeSession(stream, additionalChallenge);
            }
            else
            {
                stream.Write((byte)1);
                stream.Flush();

                return Authenticate(stream, additionalChallenge);
            }
        }

        private bool ResumeSession(Stream stream, byte[] additionalChallenge)
        {
            stream.Write((byte)16);
            byte[] aChallenge = SaltGenerator.Create(16);
            stream.Write(aChallenge);
            stream.Write((short)m_resumeTicket.Length);
            stream.Write(m_resumeTicket);
            stream.Flush();

            if (stream.ReadBoolean())
            {
                SetHashMethod((HashMethod)stream.ReadNextByte());
                byte[] bChallenge = stream.ReadBytes(stream.ReadNextByte());

                byte[] clientProof = m_hash.ComputeHash(aChallenge, bChallenge, m_sessionSecret, additionalChallenge);
                stream.Write(clientProof);
                stream.Flush();

                if (stream.ReadBoolean())
                {
                    byte[] serverProof = m_hash.ComputeHash(bChallenge, aChallenge, m_sessionSecret, additionalChallenge);
                    byte[] serverProofCheck = stream.ReadBytes(m_hash.GetDigestSize());

                    return serverProof.SecureEquals(serverProofCheck);
                }

            }
            m_sessionSecret = null;
            m_resumeTicket = null;
            return Authenticate(stream, additionalChallenge);
        }


        private bool Authenticate(Stream stream, byte[] additionalChallenge)
        {
            HashMethod passwordHashMethod = (HashMethod)stream.ReadNextByte();
            byte[] salt = stream.ReadBytes(stream.ReadNextByte());
            int iterations = stream.ReadInt32();

            SetHashMethod((HashMethod)stream.ReadNextByte());
            SetSrpStrength((SrpStrength)stream.ReadInt32());

            m_credentials.TryUpdate(passwordHashMethod, salt, iterations);

            BigInteger pubA = m_client.GenerateClientCredentials(m_hash, salt, m_credentials.UsernameBytes, m_credentials.SaltedPassword);
            byte[] pubABytes = pubA.ToPaddedArray(m_srpByteLength);

            stream.Write(pubABytes);
            stream.Flush();

            //Read from Server: B
            byte[] pubBBytes = stream.ReadBytes(m_srpByteLength);
            BigInteger pubB = new BigInteger(1, pubBBytes);

            //Calculate Session Key
            BigInteger S = m_client.CalculateSecret(m_hash, pubB);
            byte[] SBytes = S.ToPaddedArray(m_srpByteLength);

            byte[] clientProof = m_hash.ComputeHash(pubABytes, pubBBytes, SBytes, additionalChallenge);
            stream.Write(clientProof);
            stream.Flush();

            byte[] serverProof = m_hash.ComputeHash(pubBBytes, pubABytes, SBytes, additionalChallenge);

            if (stream.ReadBoolean())
            {
                byte[] serverProofCheck = stream.ReadBytes(m_hash.GetDigestSize());
                int ticketLength = stream.ReadInt16();
                if (ticketLength < 0 || ticketLength > 10000)
                    return false;

                if (serverProofCheck.SecureEquals(serverProof))
                {
                    m_resumeTicket = stream.ReadBytes(ticketLength);
                    m_sessionSecret = m_hash.ComputeHash(pubABytes, SBytes, pubBBytes).Combine(m_hash.ComputeHash(pubBBytes, SBytes, pubABytes));
                    return true;
                }
                return false;
            }
            return false;
        }


    }
}


