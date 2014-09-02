//******************************************************************************************************
//  SecureStreamClient.cs - Gbtc
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
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using GSF.Diagnostics;
using GSF.IO;
using GSF.Security.Authentication;
using Org.BouncyCastle.Crypto;

namespace GSF.Security
{
    /// <summary>
    /// Creates a secure stream that connects to a server.
    /// </summary>
    public abstract class SecureStreamClient
        : LogReporterBase
    {
        private static X509Certificate2 s_tempCert;
        private byte[] m_resumeTicket;
        private byte[] m_sessionSecret;

        static SecureStreamClient()
        {
            s_tempCert = GenerateCertificate.CreateSelfSignedCertificate("CN=Local", 256, 1024);
        }

        protected internal SecureStreamClient()
            : base(null)
        {

        }

        private bool TryConnectSsl(Stream stream, out SslStream ssl)
        {
            ssl = new SslStream(stream, true, UserCertificateValidationCallback, UserCertificateSelectionCallback, EncryptionPolicy.RequireEncryption);
            try
            {
                ssl.AuthenticateAsClient("Local", null, SslProtocols.Tls12, false);
            }
            catch (Exception ex)
            {
                Log.LogMessage(VerboseLevel.Information, "Authentication Failed", null, null, ex);
                ssl.Dispose();
                ssl = null;
                return false;
            }
            return true;
        }

        private X509Certificate UserCertificateSelectionCallback(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return s_tempCert;
        }

        private bool UserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// Attempts to authenticate the supplied stream. 
        /// </summary>
        /// <param name="stream">The stream to authenticate</param>
        /// <param name="secureStream">the secure stream if the authentication was successful.</param>
        /// <returns>true if successful</returns>
        public bool TryAuthenticate(Stream stream, out Stream secureStream)
        {
            secureStream = null;
            SslStream ssl;
            if (TryConnectSsl(stream, out ssl))
            {
                try
                {
                    byte[] certSignatures = SecureStream.ComputeCertificateChallenge(false, ssl);
                    if (m_resumeTicket != null && m_sessionSecret != null)
                    {
                        //Resume Session:
                        // C => S
                        // byte    ResumeSession
                        // byte    TicketLength
                        // byte[]  Ticket
                        // byte    ClientChallengeLength
                        // byte[]  ClientChallenge


                        byte[] clientChallenge = SaltGenerator.Create(16);
                        ssl.WriteByte((byte)AuthenticationMode.ResumeSession);
                        ssl.WriteByte((byte)m_resumeTicket.Length);
                        ssl.Write(m_resumeTicket);
                        ssl.WriteByte((byte)clientChallenge.Length);
                        ssl.Write(clientChallenge);
                        ssl.Flush();

                        // S <= C
                        // byte    HashMethod
                        // byte    ServerChallengeLength
                        // byte[]  ServerChallenge

                        HashMethod hashMethod = (HashMethod)ssl.ReadNextByte();
                        IDigest hash = Scram.CreateDigest(hashMethod);
                        byte[] serverChallenge = ssl.ReadBytes(ssl.ReadNextByte());

                        // C => S
                        // byte    ClientResponseLength
                        // byte[]  ClientChallenge

                        byte[] clientResponse = hash.ComputeHash(serverChallenge, clientChallenge, m_sessionSecret, certSignatures);
                        byte[] serverResponse = hash.ComputeHash(clientChallenge, serverChallenge, m_sessionSecret, certSignatures);

                        ssl.WriteByte((byte)clientResponse.Length);
                        ssl.Write(clientResponse);
                        ssl.Flush();

                        // S => C
                        // bool   IsSuccessful
                        // byte   ServerResponseLength
                        // byte[] ServerResponse

                        if (ssl.ReadBoolean())
                        {
                            byte[] serverResponseCheck = ssl.ReadBytes(ssl.ReadNextByte());

                            // C => S
                            // bool   IsSuccessful
                            if (serverResponse.SecureEquals(serverResponseCheck))
                            {
                                ssl.Write(true);
                                ssl.Flush();
                                secureStream = ssl;
                                return true;
                            }

                            ssl.Write(false);
                            ssl.Flush();
                        }

                        m_resumeTicket = null;
                        m_sessionSecret = null;
                    }


                    if (InternalTryAuthenticate(ssl, certSignatures))
                    {
                        m_resumeTicket = ssl.ReadBytes(ssl.ReadNextByte());
                        m_sessionSecret = ssl.ReadBytes(ssl.ReadNextByte());
                        secureStream = ssl;
                        return true;
                    }

                    ssl.Dispose();
                    return false;
                }
                catch (Exception ex)
                {
                    Log.LogMessage(VerboseLevel.Information, "Authentication Failed", null, null, ex);
                    ssl.Dispose();
                    return false;
                }
            }
            return false;

        }

        protected abstract bool InternalTryAuthenticate(Stream stream, byte[] certSignatures);

        /// <summary>
        /// Attempts to authenticate the provided stream, disposing the secure stream upon completion.
        /// </summary>
        /// <param name="stream">the stream to authenticate</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool TryAuthenticate(Stream stream)
        {
            Stream secureStream = null;
            try
            {
                return TryAuthenticate(stream, out secureStream);
            }
            finally
            {
                if (secureStream != null)
                    secureStream.Dispose();
            }
        }
        /// <summary>
        /// Authenticates the supplied stream. Returns the secure stream.
        /// </summary>
        /// <param name="stream">the stream to authenticate</param>
        /// <returns></returns>
        public Stream Authenticate(Stream stream)
        {
            Stream secureStream = null;
            try
            {
                if (TryAuthenticate(stream, out secureStream))
                {
                    return secureStream;
                }
                throw new AuthenticationException("Authentication Failed");
            }
            catch
            {
                if (secureStream != null)
                    secureStream.Dispose();
                throw;
            }
        }

    }
}
