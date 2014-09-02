//******************************************************************************************************
//  SecureStreamServer.cs - Gbtc
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
//  8/29/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using GSF.Diagnostics;
using GSF.IO;
using GSF.IO.Unmanaged;
using GSF.Security.Authentication;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace GSF.Security
{
    /// <summary>
    /// Since SecureStreamServer is a generic type,
    /// we will store the static key here instead.
    /// </summary>
    internal static class SecureStreamServerCertificate
    {
        internal static X509Certificate2 TempCertificate;

        static SecureStreamServerCertificate()
        {
            TempCertificate = GenerateCertificate.CreateSelfSignedCertificate("CN=Local", 256, 1024);
        }
    }

    /// <summary>
    /// A server host that manages a secure stream connection.
    /// </summary>
    public class SecureStreamServer<T>
        : LogReporterBase
        where T : IUserToken, new()
    {
        /// <summary>
        /// Tickets expire every 10 minutes.
        /// </summary>
        private const int TicketExpireTimeMinutes = 10;

        private Dictionary<Guid, T> m_userTokens;

        private Guid m_serverKeyName;
        private byte[] m_serverHMACKey;
        private byte[] m_serverEncryptionkey;

        //private SrpServer m_srp;
        //private ScramServer m_scram;
        //private CertificateServer m_cert;
        private IntegratedSecurityServer m_integrated;

        /// <summary>
        /// Creates a new <see cref="SecureStreamServer{T}"/>.
        /// </summary>
        public SecureStreamServer()
            : base(null)
        {
            InvalidateAllTickets();
            m_userTokens = new Dictionary<Guid, T>();
            //m_srp = new SrpServer();
            //m_scram = new ScramServer();
            //m_cert = new CertificateServer();
            m_integrated = new IntegratedSecurityServer(Log.LogSource);
        }

        /// <summary>
        /// This will change the encryption keys used to create resume tickets, thus
        /// invalidating all existing tickets.
        /// </summary>
        public void InvalidateAllTickets()
        {
            m_serverKeyName = Guid.NewGuid();
            m_serverHMACKey = SaltGenerator.Create(32);
            m_serverEncryptionkey = SaltGenerator.Create(32);
        }

        /// <summary>
        /// Adds the specified user 
        /// </summary>
        /// <param name="username">the username to add</param>
        /// <param name="userToken">The token data associated with this user</param>
        public void AddUserIntegratedSecurity(string username, T userToken)
        {
            Guid id = Guid.NewGuid();
            lock (m_userTokens)
                m_userTokens.Add(id, userToken);
            m_integrated.Users.AddUser(username, id);
        }

        /// <summary>
        /// Attempts to authenticate the stream
        /// </summary>
        /// <param name="stream">the base stream to authenticate</param>
        /// <param name="secureStream">the secure stream that is valid if the function returns true.</param>
        /// <param name="token">the user's token assocated with what user created the stream</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool TryAuthenticateAsServer(Stream stream, out Stream secureStream, out T token)
        {
            token = default(T);
            Guid userToken;

            secureStream = null;
            var ssl = new SslStream(stream, true, UserCertificateValidationCallback, UserCertificateSelectionCallback, EncryptionPolicy.RequireEncryption);
            try
            {
                try
                {
                    ssl.AuthenticateAsServer(SecureStreamServerCertificate.TempCertificate, true, SslProtocols.Tls12,
                        false);
                }
                catch (Exception ex)
                {
                    Log.LogMessage(VerboseLevel.Information, "Authentication Failed", null, null, ex);
                    ssl.Dispose();
                    return false;
                }

            TryAgain:

                byte[] certSignatures = SecureStream.ComputeCertificateChallenge(true, ssl);
                AuthenticationMode authenticationMode = (AuthenticationMode)ssl.ReadNextByte();
                switch (authenticationMode)
                {
                    //case AuthenticationMode.Srp: //SRP
                    //    m_srp.AuthenticateAsServer(ssl, certSignatures);
                    //    break;
                    case AuthenticationMode.Integrated: //Integrated
                        if (!m_integrated.TryAuthenticateAsServer(ssl, out userToken, certSignatures))
                        {
                            ssl.Dispose();
                            return false;
                        }
                        break;
                    //case AuthenticationMode.Scram: //Scram
                    //    m_scram.AuthenticateAsServer(ssl, certSignatures);
                    //    break;
                    //case AuthenticationMode.Certificate: //Certificate
                    //    m_cert.AuthenticateAsServer(ssl);
                    //    break;
                    case AuthenticationMode.ResumeSession:
                        if (TryResumeSession(ssl, certSignatures, out userToken))
                        {
                            lock (m_userTokens)
                                m_userTokens.TryGetValue(userToken, out token);
                            secureStream = ssl;
                            return true;
                        }
                        goto TryAgain;
                    default:
                        Log.LogMessage(VerboseLevel.Information, "Invalid Authentication Method",
                            authenticationMode.ToString());
                        return false;
                }

                byte[] ticket;
                byte[] secret;
                CreateResumeTicket(userToken, out ticket, out secret);
                ssl.WriteByte((byte)ticket.Length);
                ssl.Write(ticket);
                ssl.WriteByte((byte)secret.Length);
                ssl.Write(secret);
                ssl.Flush();
                lock (m_userTokens)
                    m_userTokens.TryGetValue(userToken, out token);
                secureStream = ssl;
                return true;
            }
            catch (Exception ex)
            {
                Log.LogMessage(VerboseLevel.Information, "Authentication Failed: Unknown Exception", null, null, ex);
                ssl.Dispose();
                return false;
            }
        }

        private X509Certificate UserCertificateSelectionCallback(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return SecureStreamServerCertificate.TempCertificate;
        }

        private bool UserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private bool TryResumeSession(SslStream stream, byte[] certSignatures, out Guid userToken)
        {
            //Resume Session:
            // C => S
            // byte    ResumeSession
            // byte    TicketLength
            // byte[]  Ticket
            // byte    ClientChallengeLength
            // byte[]  ClientChallenge

            // S <= C
            // byte    HashMethod
            // byte    ServerChallengeLength
            // byte[]  ServerChallenge

            IDigest hash = new Sha384Digest();
            byte[] serverChallenge = SaltGenerator.Create(16);
            stream.WriteByte((byte)HashMethod.Sha384);
            stream.WriteByte((byte)serverChallenge.Length);
            stream.Write(serverChallenge);
            stream.Flush();

            // C => S
            // byte    ClientResponseLength
            // byte[]  ClientChallenge

            byte[] ticket = stream.ReadBytes(stream.ReadNextByte());
            byte[] clientChallenge = stream.ReadBytes(stream.ReadNextByte());
            byte[] clientResponse = stream.ReadBytes(stream.ReadByte());

            byte[] secret;

            // S => C
            // bool   IsSuccessful
            // byte   ServerResponseLength
            // byte[] ServerResponse

            if (TryLoadTicket(ticket, out secret, out userToken))
            {
                byte[] clientResponseCheck = hash.ComputeHash(serverChallenge, clientChallenge, secret, certSignatures);
                if (clientResponse.SecureEquals(clientResponseCheck))
                {
                    byte[] serverResponse = hash.ComputeHash(clientChallenge, serverChallenge, secret, certSignatures);
                    stream.Write(true);
                    stream.WriteByte((byte)serverResponse.Length);
                    stream.Write(serverResponse);
                    stream.Flush();

                    if (stream.ReadBoolean())
                    {
                        return true;
                    }
                    return false;
                }
            }
            stream.Write(false);
            userToken = default(Guid);
            return false;
        }

        private unsafe bool TryLoadTicket(byte[] ticket, out byte[] sessionSecret, out Guid userToken)
        {
            const int encryptedLength = 32 + 16;
            const int ticketSize = 1 + 16 + 8 + 16 + encryptedLength + 32;

            userToken = default(Guid);
            sessionSecret = null;

            //Ticket Structure:
            //  byte      TicketVersion = 1
            //  Guid      ServerKeyName
            //  DateTime  AuthenticationTime
            //  byte[16]  IV
            //  byte[]    Encrypted Session Data 32+16
            //  byte[32]  HMAC (Sha2-256)

            if (ticket == null || ticket.Length != ticketSize)
                return false;

            fixed (byte* lp = ticket)
            {
                var stream = new BinaryStreamPointerWrapper(lp, ticket.Length);
                if (stream.ReadUInt8() != 1)
                    return false;

                if (!m_serverKeyName.SecureEquals(stream.ReadGuid()))
                    return false;


                long currentTime = DateTime.UtcNow.Ticks;
                long issueTime = stream.ReadInt64();


                if (issueTime <= currentTime)
                {
                    //Issue time is before current time.
                    if (currentTime - issueTime > TimeSpan.TicksPerMinute * TicketExpireTimeMinutes)
                    {
                        return false;
                    }
                }
                else
                {
                    //For some reason, the issue time is after the current time. 
                    //This could be due to a clock sync on the server after the ticket was issued. 
                    //Allow up to 1 minute of clock skew
                    if (issueTime - currentTime > TimeSpan.TicksPerMinute)
                    {
                        return false;
                    }
                }

                byte[] initializationVector = stream.ReadBytes(16);

                //Verify the signature if everything else looks good.
                //This is last because it is the most computationally complex.
                //This limits denial of service attackes.
                byte[] hmac = HMAC<Sha256Digest>.Compute(m_serverHMACKey, ticket, 0, ticket.Length - 32);
                if (!hmac.SecureEquals(ticket, ticket.Length - 32, 32))
                    return false;

                byte[] encryptedData = stream.ReadBytes(encryptedLength);
                var aes = new RijndaelManaged();
                aes.Key = m_serverEncryptionkey;
                aes.IV = initializationVector;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;
                var decrypt = aes.CreateDecryptor();
                encryptedData = decrypt.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

                sessionSecret = new byte[32];
                Array.Copy(encryptedData, 0, sessionSecret, 0, 32);
                userToken = encryptedData.ToRfcGuid(32);

                return true;
            }
        }

        private unsafe void CreateResumeTicket(Guid userToken, out byte[] ticket, out byte[] sessionSecret)
        {
            //Ticket Structure:
            //  byte      TicketVersion = 1
            //  Guid      ServerKeyName
            //  DateTime  AuthenticationTime
            //  byte[16]  IV
            //  byte[]    Encrypted Session Data 32+16
            //  byte[32]  HMAC (Sha2-256)

            const int encryptedLength = 32 + 16;
            const int ticketSize = 1 + 16 + 8 + 16 + encryptedLength + 32;

            byte[] initializationVector = SaltGenerator.Create(16);
            sessionSecret = SaltGenerator.Create(32);
            byte[] userTokenBytes = userToken.ToRfcBytes();
            byte[] dataToEncrypt = new byte[encryptedLength];
            sessionSecret.CopyTo(dataToEncrypt, 0);
            userTokenBytes.CopyTo(dataToEncrypt, sessionSecret.Length);
            ticket = new byte[ticketSize];

            var aes = new RijndaelManaged();
            aes.Key = m_serverEncryptionkey;
            aes.IV = initializationVector;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;
            var decrypt = aes.CreateEncryptor();
            var encryptedData = decrypt.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);

            fixed (byte* lp = ticket)
            {
                var stream = new BinaryStreamPointerWrapper(lp, ticket.Length);
                stream.Write((byte)1);
                stream.Write(m_serverKeyName);
                stream.Write(DateTime.UtcNow.RoundDownToNearestMinute());
                stream.Write(initializationVector);
                stream.Write(encryptedData); //Encrypted data, 32 byte session key, n byte user token
                stream.Write(HMAC<Sha256Digest>.Compute(m_serverHMACKey, ticket, 0, ticket.Length - 32));
            }
        }
    }
}
