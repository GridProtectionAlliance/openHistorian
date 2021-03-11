//******************************************************************************************************
//  SecureStreamClient.cs - Gbtc
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
#if !SQLCLR
using GSF.Security.Authentication;
using Org.BouncyCastle.Crypto;
#endif

namespace GSF.Security
{
    /// <summary>
    /// Creates a secure stream that connects to a server.
    /// </summary>
    public abstract class SecureStreamClientBase
        : DisposableLoggingClassBase
    {
        private static readonly X509Certificate2 s_tempCert;
        private byte[] m_resumeTicket;
        private byte[] m_sessionSecret;

        static SecureStreamClientBase()
        {
#if !SQLCLR
            try
            {
                s_tempCert = GenerateCertificate.CreateSelfSignedCertificate("CN=Local", 256, 1024);
            }
            catch
            {
            }
#endif
        }

        protected internal SecureStreamClientBase()
            : base(MessageClass.Component)
        {

        }

        private bool TryConnectSsl(Stream stream, out SslStream ssl)
        {
            ssl = new SslStream(stream, false, UserCertificateValidationCallback, UserCertificateSelectionCallback, EncryptionPolicy.RequireEncryption);
            try
            {
                ssl.AuthenticateAsClient("Local", null, SslProtocols.Tls12, false);
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
        /// <param name="useSsl"></param>
        /// <param name="secureStream">the secure stream if the authentication was successful.</param>
        /// <returns>true if successful</returns>
        public bool TryAuthenticate(Stream stream, bool useSsl, out Stream secureStream)
        {
            secureStream = null;
            SslStream ssl = null;
            Stream stream2;
            byte[] certSignatures;

            if (useSsl)
            {
                if (!TryConnectSsl(stream, out ssl))
                    return false;
                stream2 = ssl;
                certSignatures = SecureStream.ComputeCertificateChallenge(false, ssl);
            }
            else
            {
                stream2 = stream;
                certSignatures = new byte[0];
            }

            try
            {

                try
                {
                    if (TryResumeSession(ref secureStream, stream2, certSignatures))
                        return true;
                }
                catch (FileNotFoundException ex)
                {
                    Log.Publish(MessageLevel.Info, "Bouncy Castle dll is missing! Oh No!", null, null, ex);
                }


                if (InternalTryAuthenticate(stream2, certSignatures))
                {
                    if (stream2.ReadBoolean())
                    {
                        m_resumeTicket = stream2.ReadBytes(stream2.ReadNextByte());
                        m_sessionSecret = stream2.ReadBytes(stream2.ReadNextByte());
                    }
                    secureStream = stream2;
                    return true;
                }

                if (ssl != null)
                    ssl.Dispose();
                return false;
            }
            catch (Exception ex)
            {
                Log.Publish(MessageLevel.Info, "Authentication Failed", null, null, ex);
                if (ssl != null)
                    ssl.Dispose();
                return false;
            }

        }

        private bool TryResumeSession(ref Stream secureStream, Stream stream2, byte[] certSignatures)
        {
#if SQLCLR
            return false;
#else
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
                stream2.WriteByte((byte)AuthenticationMode.ResumeSession);
                stream2.WriteByte((byte)m_resumeTicket.Length);
                stream2.Write(m_resumeTicket);
                stream2.WriteByte((byte)clientChallenge.Length);
                stream2.Write(clientChallenge);
                stream2.Flush();

                // S <= C
                // byte    HashMethod
                // byte    ServerChallengeLength
                // byte[]  ServerChallenge

                HashMethod hashMethod = (HashMethod)stream2.ReadNextByte();
                IDigest hash = Scram.CreateDigest(hashMethod);
                byte[] serverChallenge = stream2.ReadBytes(stream2.ReadNextByte());

                // C => S
                // byte    ClientResponseLength
                // byte[]  ClientChallenge

                byte[] clientResponse = hash.ComputeHash(serverChallenge, clientChallenge, m_sessionSecret, certSignatures);
                byte[] serverResponse = hash.ComputeHash(clientChallenge, serverChallenge, m_sessionSecret, certSignatures);

                stream2.WriteByte((byte)clientResponse.Length);
                stream2.Write(clientResponse);
                stream2.Flush();

                // S => C
                // bool   IsSuccessful
                // byte   ServerResponseLength
                // byte[] ServerResponse

                if (stream2.ReadBoolean())
                {
                    byte[] serverResponseCheck = stream2.ReadBytes(stream2.ReadNextByte());

                    // C => S
                    // bool   IsSuccessful
                    if (serverResponse.SecureEquals(serverResponseCheck))
                    {
                        stream2.Write(true);
                        stream2.Flush();
                        secureStream = stream2;
                        return true;
                    }

                    stream2.Write(false);
                    stream2.Flush();
                }

                m_resumeTicket = null;
                m_sessionSecret = null;
            }
            return false;
#endif
        }

        protected abstract bool InternalTryAuthenticate(Stream stream, byte[] certSignatures);

        /// <summary>
        /// Attempts to authenticate the provided stream, disposing the secure stream upon completion.
        /// </summary>
        /// <param name="stream">the stream to authenticate</param>
        /// <param name="useSsl">gets if SSL will be used to authenticate</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool TryAuthenticate(Stream stream, bool useSsl = true)
        {
            Stream secureStream = null;
            try
            {
                return TryAuthenticate(stream, useSsl, out secureStream);
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
        /// <param name="useSsl">gets if SSL will be used to authenticate</param>
        /// <returns></returns>
        public Stream Authenticate(Stream stream, bool useSsl = true)
        {
            Stream secureStream = null;
            try
            {
                if (TryAuthenticate(stream, useSsl, out secureStream))
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
