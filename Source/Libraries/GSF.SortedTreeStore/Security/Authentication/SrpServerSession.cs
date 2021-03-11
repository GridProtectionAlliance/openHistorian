//******************************************************************************************************
//  SrpServerSession.cs - Gbtc
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
//  07/27/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//  04/11/2017 - J. Ritchie Carroll
//       Modified code to use FIPS compatible security algorithms when required.
//
//******************************************************************************************************

using System;
using System.IO;
using System.Security.Cryptography;
using GSF.IO.Unmanaged;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement.Srp;
using Org.BouncyCastle.Crypto.Digests;
using GSF.IO;
using GSF.Security.Cryptography;
using Org.BouncyCastle.Math;

namespace GSF.Security.Authentication
{
    /// <summary>
    /// Provides simple password based authentication that uses Secure Remote Password.
    /// </summary>
    public class SrpServerSession
    {
        /// <summary>
        /// The session secret that is used to generate keys.
        /// </summary>
        public byte[] SessionSecret;

        private const HashMethod PasswordHashMethod = HashMethod.Sha512;
        private const HashMethod SrpHashMethod = HashMethod.Sha512;
        private readonly SrpUserCredential m_user;

        /// <summary>
        /// Creates a new <see cref="SrpServerSession"/> that will authenticate a stream.
        /// </summary>
        /// <param name="user">The user that will be authenticated.</param>
        public SrpServerSession(SrpUserCredential user)
        {
            m_user = user;
        }

        /// <summary>
        /// Attempts to authenticate the provided stream.
        /// </summary>
        /// <param name="stream">the stream to authenticate</param>
        /// <param name="additionalChallenge">Any additional challenge bytes.</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool TryAuthenticate(Stream stream, byte[] additionalChallenge)
        {
            // Header
            //  C => S
            //  byte    Mode = (1: New, 2: Resume)

            byte mode = stream.ReadNextByte();
            Sha512Digest hash = new Sha512Digest();

            switch (mode)
            {
                case 1:
                    return StandardAuthentication(hash, stream, additionalChallenge);
                case 2: //resume ticket
                    return ResumeTicket(hash, stream, additionalChallenge);
                default:
                    return false;
            }
        }

        private bool StandardAuthentication(IDigest hash, Stream stream, byte[] additionalChallenge)
        {
            // Authenticate (If mode = 1)
            //  C <= S
            //  byte    PasswordHashMethod
            //  byte    SaltLength
            //  byte[]  Salt
            //  int     Iterations
            //  byte    SrpHashMethod
            //  int     Bit Strength
            //  byte[]  Public B (Size equal to SRP Length)
            //  C => S  
            //  byte[]  Public A (Size equal to SRP Length)
            //  byte[]  Client Proof: H(Public A | Public B | SessionKey)
            //  C <= S
            //  Bool    Success (if false, done)
            //  byte[]  Server Proof: H(Public B | Public A | SessionKey)

            int srpNumberLength = (int)m_user.SrpStrength >> 3;
            stream.WriteByte((byte)PasswordHashMethod);
            stream.WriteByte((byte)m_user.Salt.Length);
            stream.Write(m_user.Salt);
            stream.Write(m_user.Iterations);

            stream.WriteByte((byte)SrpHashMethod);
            stream.Write((int)m_user.SrpStrength);
            stream.Flush(); //since computing B takes a long time. Go ahead and flush

            SrpConstants param = SrpConstants.Lookup(m_user.SrpStrength);
            Srp6Server server = new Srp6Server(param, m_user.VerificationInteger);
            BigInteger pubB = server.GenerateServerCredentials();

            byte[] pubBBytes = pubB.ToPaddedArray(srpNumberLength);
            stream.Write(pubBBytes);
            stream.Flush();

            //Read from client: A
            byte[] pubABytes = stream.ReadBytes(srpNumberLength);
            BigInteger pubA = new BigInteger(1, pubABytes);

            //Calculate Session Key
            BigInteger S = server.CalculateSecret(hash, pubA);
            byte[] SBytes = S.ToPaddedArray(srpNumberLength);


            byte[] clientProofCheck = hash.ComputeHash(pubABytes, pubBBytes, SBytes, additionalChallenge);
            byte[] serverProof = hash.ComputeHash(pubBBytes, pubABytes, SBytes, additionalChallenge);
            byte[] clientProof = stream.ReadBytes(hash.GetDigestSize());

            if (clientProof.SecureEquals(clientProofCheck))
            {
                stream.Write(true);
                stream.Write(serverProof);
                stream.Flush();

                byte[] K = hash.ComputeHash(pubABytes, SBytes, pubBBytes).Combine(hash.ComputeHash(pubBBytes, SBytes, pubABytes));
                byte[] ticket = CreateSessionData(K, m_user);
                SessionSecret = K;
                stream.Write((short)ticket.Length);
                stream.Write(ticket);
                stream.Flush();
                return true;
            }
            stream.Write(false);
            stream.Flush();

            return false;
        }

        private bool ResumeTicket(IDigest hash, Stream stream, byte[] additionalChallenge)
        {
            // Successful Resume Session (If mode = 1)
            //  C => S
            //  byte    ChallengeLength
            //  byte[]  A = Challenge
            //  int16   TicketLength
            //  byte[]  Ticket
            //  C <= S
            //  bool    IsSuccess = true
            //  byte    HashMethod
            //  byte    ChallengeLength
            //  byte[]  B = Challenge
            //  C => S  
            //  byte[]  M1 = H(A | B | SessionKey)
            //  Bool    Success (if false, done)
            //  C <= S
            //  byte[]  M2 = H(B | A | SessionKey)

            // Failed Resume Session
            //  C => S
            //  byte    ChallengeLength
            //  byte[]  A = Challenge
            //  int16   TicketLength
            //  byte[]  Ticket
            //  C <= S
            //  bool    IsSuccess = false
            //  Goto Authenticate Code

            byte[] a = stream.ReadBytes(stream.ReadNextByte());
            int ticketLength = stream.ReadInt16();
            if (ticketLength < 0 || ticketLength > 10000)
                return false;

            byte[] ticket = stream.ReadBytes(ticketLength);

            if (TryLoadTicket(ticket, m_user, out SessionSecret))
            {
                stream.Write(true);
                stream.WriteByte((byte)SrpHashMethod);
                byte[] b = SaltGenerator.Create(16);
                stream.WriteByte(16);
                stream.Write(b);
                stream.Flush();

                byte[] clientProofCheck = hash.ComputeHash(a, b, SessionSecret, additionalChallenge);
                byte[] serverProof = hash.ComputeHash(b, a, SessionSecret, additionalChallenge);
                byte[] clientProof = stream.ReadBytes(hash.GetDigestSize());

                if (clientProof.SecureEquals(clientProofCheck))
                {
                    stream.Write(true);
                    stream.Write(serverProof);
                    stream.Flush();
                    return true;
                }
                stream.Write(false);
                return false;
            }
            stream.Write(false);
            return StandardAuthentication(hash, stream, additionalChallenge);
        }

        private static unsafe byte[] CreateSessionData(byte[] sessionSecret, SrpUserCredential user)
        {
            byte[] initializationVector = SaltGenerator.Create(16);
            int len = sessionSecret.Length;
            int blockLen = (len + 15) & ~15; //Add 15, then round down. (Effectively rounds up to the nearest 128 bit boundary).
            byte[] dataToEncrypt = new byte[blockLen];
            sessionSecret.CopyTo(dataToEncrypt, 0);

            //fill the remainder of the block with random bits
            if (len != blockLen)
            {
                SaltGenerator.Create(blockLen - len).CopyTo(dataToEncrypt, len);
            }

            byte[] ticket = new byte[1 + 16 + 8 + 16 + 2 + blockLen + 32];

            using (Aes aes = Cipher.CreateAes())
            {
                aes.Key = user.ServerEncryptionkey;
                aes.IV = initializationVector;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;
                ICryptoTransform decrypt = aes.CreateEncryptor();
                byte[] encryptedData = decrypt.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);

                fixed (byte* lp = ticket)
                {
                    using (BinaryStreamPointerWrapper stream = new BinaryStreamPointerWrapper(lp, ticket.Length))
                    {
                        stream.Write((byte)1);
                        stream.Write((short)len);
                        stream.Write(user.ServerKeyName);
                        stream.Write(DateTime.UtcNow);
                        stream.Write(initializationVector);
                        stream.Write(encryptedData);
                        stream.Write(HMAC<Sha256Digest>.Compute(user.ServerHMACKey, ticket, 0, ticket.Length - 32));
                    }
                }
            }
            return ticket;
        }

        /// <summary>
        /// Attempts to load the session resume ticket.
        /// </summary>
        /// <param name="ticket">The serialized data for the key</param>
        /// <param name="user">The user's credentials so the proper encryption key can be used</param>
        /// <param name="sessionSecret">the session secret decoded if successful. null otherwise</param>
        /// <returns>
        /// True if the ticket is authentic
        /// </returns>
        private static unsafe bool TryLoadTicket(byte[] ticket, SrpUserCredential user, out byte[] sessionSecret)
        {
            sessionSecret = null;

            //Ticket Structure:
            //  byte      TicketVersion = 1
            //  int16     Username Length
            //  int16     Session Key Length 
            //  Guid      ServerKeyName
            //  DateTime  AuthenticationTime
            //  byte[16]  IV
            //  byte[]    Username
            //  byte[]    Encrypted Session Data (Round [Session Key Length] up to a 128 bit number to get the length of the encrypted data)
            //  byte[32]  HMAC (Sha2-256)

            if (ticket is null || ticket.Length < 1 + 16 + 8 + 16 + 2 + 32)
                return false;

            fixed (byte* lp = ticket)
            {
                BinaryStreamPointerWrapper stream = new BinaryStreamPointerWrapper(lp, ticket.Length);
                if (stream.ReadUInt8() != 1)
                    return false;

                int sessionKeyLength = stream.ReadUInt16();
                if (sessionKeyLength < 0 || sessionKeyLength > 1024) //Max session key is 8192 SRP.
                    return false;

                int encryptedDataLength = (sessionKeyLength + 15) & ~15; //Add 15, then round down. (Effectively rounds up to the nearest 128 bit boundary).
                if (ticket.Length != 1 + 2 + 16 + 8 + 16 + encryptedDataLength + 32)
                    return false;

                if (!user.ServerKeyName.SecureEquals(stream.ReadGuid()))
                    return false;

                long maxTicketAge = DateTime.UtcNow.Ticks + TimeSpan.TicksPerMinute * 10; //Allows for time syncing issues that might move the server's time back by a few minutes.
                long minTicketAge = maxTicketAge - TimeSpan.TicksPerDay;

                long issueTime = stream.ReadInt64();
                if (issueTime < minTicketAge || issueTime > maxTicketAge)
                    return false;

                byte[] initializationVector = stream.ReadBytes(16);

                //Verify the signature if everything else looks good.
                //This is last because it is the most computationally complex.
                //This limits denial of service attacks.
                byte[] hmac = HMAC<Sha256Digest>.Compute(user.ServerHMACKey, ticket, 0, ticket.Length - 32);
                if (!hmac.SecureEquals(ticket, ticket.Length - 32, 32))
                    return false;

                byte[] encryptedData = stream.ReadBytes(encryptedDataLength);

                using (Aes aes = Cipher.CreateAes())
                {
                    aes.Key = user.ServerEncryptionkey;
                    aes.IV = initializationVector;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.None;
                    ICryptoTransform decrypt = aes.CreateDecryptor();
                    sessionSecret = decrypt.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                }

                return true;
            }
        }
    }
}