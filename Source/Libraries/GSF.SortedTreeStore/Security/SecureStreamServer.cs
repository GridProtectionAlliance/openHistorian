//******************************************************************************************************
//  SecureStreamServer.cs - Gbtc
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
//  08/29/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//  04/11/2017 - J. Ritchie Carroll
//       Modified code to use FIPS compatible security algorithms when required.
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
using GSF.Security.Cryptography;
#if !SQLCLR
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
#endif

namespace GSF.Security
{
    /// <summary>
    /// Since SecureStreamServer is a generic type,
    /// we will store the static key here instead.
    /// </summary>
    internal static class SecureStreamServerCertificate
    {
        /// <summary>
        /// A RSA-1024 SHA-256 certificate. It takes about 250ms to generate this certificate.
        /// </summary>
        internal static X509Certificate2 TempCertificate;

        static SecureStreamServerCertificate()
        {
#if !SQLCLR
            TempCertificate = GenerateCertificate.CreateSelfSignedCertificate("CN=Local", 256, 1024);
#endif
        }
    }

    /// <summary>
    /// A server host that manages a secure stream connection.
    /// This class is thread safe and can negotiate streams simultaneous.
    /// </summary>
    public class SecureStreamServer<T>
        : DisposableLoggingClassBase
        where T : IUserToken, new()
    {

        private class State
        {
            public Guid ServerKeyName;
            public byte[] ServerHMACKey;
            public byte[] ServerEncryptionkey;
            public bool ContainsDefaultCredentials;
            public Guid DefaultUserToken;

            public State Clone()
            {
                return (State)MemberwiseClone();
            }
        }

        /// <summary>
        /// Tickets expire every 10 minutes.
        /// </summary>
        private const int TicketExpireTimeMinutes = 10;
        private readonly Dictionary<Guid, T> m_userTokens;
        //private SrpServer m_srp;
        //private ScramServer m_scram;
        //private CertificateServer m_cert;
        private readonly IntegratedSecurityServer m_integrated;
        private State m_state;
        private readonly object m_syncRoot;

        /// <summary>
        /// Creates a new <see cref="SecureStreamServer{T}"/>.
        /// </summary>
        public SecureStreamServer()
            : base(MessageClass.Component)
        {
            m_syncRoot = new object();
            m_state = new State();
            m_state.ContainsDefaultCredentials = false;
            InvalidateAllTickets();
            m_userTokens = new Dictionary<Guid, T>();
            //m_srp = new SrpServer();
            //m_scram = new ScramServer();
            //m_cert = new CertificateServer();
            m_integrated = new IntegratedSecurityServer();
        }

        /// <summary>
        /// This will change the encryption keys used to create resume tickets, thus
        /// invalidating all existing tickets.
        /// </summary>
        public void InvalidateAllTickets()
        {
            lock (m_syncRoot)
            {
                State state = m_state.Clone();
                state.ServerKeyName = Guid.NewGuid();
                state.ServerHMACKey = SaltGenerator.Create(32);
                state.ServerEncryptionkey = SaltGenerator.Create(32);
                m_state = state;
            }
        }

        /// <summary>
        /// Adds the default user credential if the user logs in with no credentials specified.
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="userToken"></param>
        public void SetDefaultUser(bool enabled, T userToken)
        {
            lock (m_syncRoot)
            {
                State state = m_state.Clone();
                m_userTokens.Remove(state.DefaultUserToken);
                state.DefaultUserToken = Guid.NewGuid();
                state.ContainsDefaultCredentials = enabled;
                m_userTokens.Add(state.DefaultUserToken, userToken);
                m_state = state;
            }
        }

        /// <summary>
        /// Adds the specified user 
        /// </summary>
        /// <param name="username">the username to add</param>
        /// <param name="userToken">The token data associated with this user</param>
        public void AddUserIntegratedSecurity(string username, T userToken)
        {
            Guid id = Guid.NewGuid();
            lock (m_syncRoot)
            {
                m_userTokens.Add(id, userToken);
                m_integrated.Users.AddUser(username, id);
            }
        }

        private bool TryConnectSsl(Stream stream, out SslStream ssl)
        {
            ssl = new SslStream(stream, false, UserCertificateValidationCallback, UserCertificateSelectionCallback, EncryptionPolicy.RequireEncryption);
            try
            {
                ssl.AuthenticateAsServer(SecureStreamServerCertificate.TempCertificate, true, SslProtocols.Tls12, false);
            }
            catch (Exception ex)
            {
                Log.Publish(MessageLevel.Info, "Authentication Failed", null, null, ex);
                ssl.Dispose();
                ssl = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Attempts to authenticate the stream
        /// </summary>
        /// <param name="stream">the base stream to authenticate</param>
        /// <param name="useSsl">gets if ssl should be used</param>
        /// <param name="secureStream">the secure stream that is valid if the function returns true.</param>
        /// <param name="token">the user's token associated with what user created the stream</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool TryAuthenticateAsServer(Stream stream, bool useSsl, out Stream secureStream, out T token)
        {
            token = default;
            secureStream = null;
            SslStream ssl = null;

            try
            {
                Stream stream2;
                byte[] certSignatures;
                if (useSsl)
                {
                    if (!TryConnectSsl(stream, out ssl))
                        return false;
                    stream2 = ssl;
                    certSignatures = SecureStream.ComputeCertificateChallenge(true, ssl);
                }
                else
                {
                    certSignatures = new byte[0];
                    stream2 = stream;
                }

            TryAgain:

                State state = m_state;
                AuthenticationMode authenticationMode = (AuthenticationMode)stream2.ReadNextByte();
                Guid userToken;
                switch (authenticationMode)
                {
                    case AuthenticationMode.None:
                        if (!state.ContainsDefaultCredentials)
                        {
                            stream2.Write(false);
                            if (ssl != null)
                                ssl.Dispose();
                            return false;
                        }
                        stream2.Write(true);
                        userToken = state.DefaultUserToken;
                        break;
                    //case AuthenticationMode.Srp: //SRP
                    //    m_srp.AuthenticateAsServer(ssl, certSignatures);
                    //    break;
                    case AuthenticationMode.Integrated: //Integrated
                        if (!m_integrated.TryAuthenticateAsServer(stream2, out userToken, certSignatures))
                        {
                            if (ssl != null)
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
                        if (TryResumeSession(stream2, certSignatures, out userToken))
                        {
                            lock (m_syncRoot)
                            {
                                m_userTokens.TryGetValue(userToken, out token);
                            }
                            secureStream = stream2;
                            return true;
                        }
                        goto TryAgain;
                    default:
                        Log.Publish(MessageLevel.Info, "Invalid Authentication Method",
                            authenticationMode.ToString());
                        return false;
                }


                stream2.Write(false);
                stream2.Flush();

                //ToDo: Support resume tickets
                //byte[] ticket;
                //byte[] secret;
                //CreateResumeTicket(userToken, out ticket, out secret);
                //stream2.WriteByte((byte)ticket.Length);
                //stream2.Write(ticket);
                //stream2.WriteByte((byte)secret.Length);
                //stream2.Write(secret);
                //stream2.Flush();
                lock (m_syncRoot)
                {
                    m_userTokens.TryGetValue(userToken, out token);
                }
                secureStream = stream2;
                return true;
            }
            catch (Exception ex)
            {
                Log.Publish(MessageLevel.Info, "Authentication Failed: Unknown Exception", null, null, ex);
                if (ssl != null)
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

        private bool TryResumeSession(Stream stream, byte[] certSignatures, out Guid userToken)
        {
#if SQLCLR
            userToken = Guid.Empty;
            return false;
#else
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

            // S => C
            // bool   IsSuccessful
            // byte   ServerResponseLength
            // byte[] ServerResponse

            if (TryLoadTicket(ticket, out byte[] secret, out userToken))
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
            userToken = default;
            return false;
#endif
        }

        private unsafe bool TryLoadTicket(byte[] ticket, out byte[] sessionSecret, out Guid userToken)
        {
#if SQLCLR
            sessionSecret = null;
            userToken = Guid.Empty;
            return false;
#else
            State state = m_state;
            const int encryptedLength = 32 + 16;
            const int ticketSize = 1 + 16 + 8 + 16 + encryptedLength + 32;

            userToken = default;
            sessionSecret = null;

            //Ticket Structure:
            //  byte      TicketVersion = 1
            //  Guid      ServerKeyName
            //  DateTime  AuthenticationTime
            //  byte[16]  IV
            //  byte[]    Encrypted Session Data 32+16
            //  byte[32]  HMAC (Sha2-256)

            if (ticket is null || ticket.Length != ticketSize)
                return false;

            fixed (byte* lp = ticket)
            {
                BinaryStreamPointerWrapper stream = new BinaryStreamPointerWrapper(lp, ticket.Length);
                if (stream.ReadUInt8() != 1)
                    return false;

                if (!state.ServerKeyName.SecureEquals(stream.ReadGuid()))
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
                //This limits denial of service attacks.
                byte[] hmac = HMAC<Sha256Digest>.Compute(state.ServerHMACKey, ticket, 0, ticket.Length - 32);
                if (!hmac.SecureEquals(ticket, ticket.Length - 32, 32))
                    return false;

                byte[] encryptedData = stream.ReadBytes(encryptedLength);

                using (Aes aes = Cipher.CreateAes())
                {
                    aes.Key = state.ServerEncryptionkey;
                    aes.IV = initializationVector;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.None;
                    ICryptoTransform decrypt = aes.CreateDecryptor();
                    encryptedData = decrypt.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

                    sessionSecret = new byte[32];
                    Array.Copy(encryptedData, 0, sessionSecret, 0, 32);
                    userToken = encryptedData.ToRfcGuid(32);
                }

                return true;
            }
#endif
        }

        private unsafe void CreateResumeTicket(Guid userToken, out byte[] ticket, out byte[] sessionSecret)
        {
#if SQLCLR
            ticket = null;
            sessionSecret = null;
#else
            State state = m_state;

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

            using (Aes aes = Cipher.CreateAes())
            {
                aes.Key = state.ServerEncryptionkey;
                aes.IV = initializationVector;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;
                ICryptoTransform decrypt = aes.CreateEncryptor();
                byte[] encryptedData = decrypt.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);

                fixed (byte* lp = ticket)
                {
                    BinaryStreamPointerWrapper stream = new BinaryStreamPointerWrapper(lp, ticket.Length);
                    stream.Write((byte)1);
                    stream.Write(state.ServerKeyName);
                    stream.Write(DateTime.UtcNow.RoundDownToNearestMinute());
                    stream.Write(initializationVector);
                    stream.Write(encryptedData); //Encrypted data, 32 byte session key, n byte user token
                    stream.Write(HMAC<Sha256Digest>.Compute(state.ServerHMACKey, ticket, 0, ticket.Length - 32));
                }
            }
#endif
        }
    }
}
