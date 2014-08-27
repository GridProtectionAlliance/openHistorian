//******************************************************************************************************
//  SrpClient.cs - Gbtc
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

using System;
using System.IO;
using GSF.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement.Srp;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math;

namespace GSF.Security
{
    /// <summary>
    /// Provides simple password based authentication that uses Secure Remote Password.
    /// </summary>
    public class SrpClient
    {
        private PBDKFCredentials m_credentials;

        //The SRP Mechanism
        private int m_srpByteLength;
        private SrpStrength m_strength;
        private SrpConstants m_param;
        Srp6Client m_client;
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
            if (username == null)
                throw new ArgumentNullException("username");
            if (password == null)
                throw new ArgumentNullException("username");

            m_credentials = new PBDKFCredentials(username, password);
            if (m_credentials.UsernameBytes.Length > 1024)
                throw new ArgumentException("Username cannot consume more than 1024 bytes encoded as UTF8", "username");
        }

        void SetSrpStrength(SrpStrength strength)
        {
            m_strength = strength;
            m_srpByteLength = ((int)strength) >> 3;
            m_param = SrpConstants.Lookup(m_strength);
            m_client = new Srp6Client(m_param);
        }

        void SetHashMethod(HashMethod hashMethod)
        {
            switch (hashMethod)
            {
                case HashMethod.Sha1:
                    m_hash = new Sha1Digest();
                    break;
                case HashMethod.Sha256:
                    m_hash = new Sha256Digest();
                    break;
                case HashMethod.Sha384:
                    m_hash = new Sha384Digest();
                    break;
                case HashMethod.Sha512:
                    m_hash = new Sha512Digest();
                    break;
                default:
                    throw new Exception("Unsupported Hash Method");
            }
        }

        public bool AuthenticateAsClient(Stream stream)
        {
            stream.Write((short)m_credentials.UsernameBytes.Length);
            stream.Write(m_credentials.UsernameBytes);

            if (m_resumeTicket != null)
            {
                //resume
                stream.Write((byte)2);
                stream.Flush();

                return ResumeSession(stream);
            }
            else
            {
                stream.Write((byte)1);
                stream.Flush();

                return Authenticate(stream);
            }
        }

        private bool ResumeSession(Stream stream)
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

                byte[] clientProof = m_hash.ComputeHash(aChallenge, bChallenge, m_sessionSecret);
                stream.Write(clientProof);
                stream.Flush();

                if (stream.ReadBoolean())
                {
                    byte[] serverProof = m_hash.ComputeHash(bChallenge, aChallenge, m_sessionSecret);
                    byte[] serverProofCheck = stream.ReadBytes(m_hash.GetDigestSize());

                    return serverProof.SecureEquals(serverProofCheck);
                }

            }
            m_sessionSecret = null;
            m_resumeTicket = null;
            return Authenticate(stream);
        }


        private bool Authenticate(Stream stream)
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

            byte[] clientProof = m_hash.ComputeHash(pubABytes, pubBBytes, SBytes);
            stream.Write(clientProof);
            stream.Flush();

            byte[] serverProof = m_hash.ComputeHash(pubBBytes, pubABytes, SBytes);

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


