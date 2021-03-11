//******************************************************************************************************
//  IntegratedSecurityClient.cs - Gbtc
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
using System.Net;
using System.Net.Security;
using System.Security.Principal;
using GSF.Diagnostics;
using GSF.IO;

namespace GSF.Security.Authentication
{
    /// <summary>
    /// Uses windows integrated security to authentication.
    /// This uses NTLM in non-domain environments 
    /// and Kerberos in domain environments.
    /// </summary>
    public class IntegratedSecurityClient 
        : DisposableLoggingClassBase
    {
        private readonly NetworkCredential m_credentials;

        /// <summary>
        /// Uses the default credentials of the user to authenticate
        /// </summary>
        public IntegratedSecurityClient()
            : base(MessageClass.Component)
        {
            m_credentials = CredentialCache.DefaultNetworkCredentials;
        }

        /// <summary>
        /// Uses the specified username and password to authenticate.
        /// </summary>
        /// <param name="username">The username to use</param>
        /// <param name="password">the password to use</param>
        /// <param name="domain">the domain to long in as.</param>
        public IntegratedSecurityClient(string username, string password, string domain)
            : base(MessageClass.Component)
        {
            m_credentials = new NetworkCredential(username, password, domain);
        }

        /// <summary>
        /// Authenticates the client using the supplied stream.
        /// </summary>
        /// <param name="stream">the stream to use to authenticate the connection.</param>
        /// <param name="additionalChallenge">Additional data that much match between the client and server
        /// for the connection to succeed.</param>
        /// <returns>
        /// True if authentication succeded, false otherwise.
        /// </returns>
        public bool TryAuthenticateAsClient(Stream stream, byte[] additionalChallenge = null)
        {
            if (additionalChallenge is null)
                additionalChallenge = new byte[] { };
            if (additionalChallenge.Length > short.MaxValue)
                throw new ArgumentOutOfRangeException("additionalChallenge","Must be less than 32767 bytes");

            using (NegotiateStream negotiateStream = new NegotiateStream(stream, true))
            {
                try
                {
                    negotiateStream.AuthenticateAsClient(m_credentials, string.Empty, ProtectionLevel.EncryptAndSign, TokenImpersonationLevel.Identification);
                }
                catch (Exception ex)
                {
                    Log.Publish(MessageLevel.Info, "Security Login Failed", "Attempting an integrated security login failed", null, ex);
                    return false;
                }

                //Exchange the challenge data.
                //Since NegotiateStream is already a trusted stream
                //Simply writing the raw is as secure as creating a challenge response
                negotiateStream.Write((short)additionalChallenge.Length);
                if (additionalChallenge.Length > 0)
                {
                    negotiateStream.Write(additionalChallenge);
                }
                negotiateStream.Flush();

                int len = negotiateStream.ReadInt16();
                if (len < 0)
                {
                    Log.Publish(MessageLevel.Info, "Security Login Failed", "Attempting an integrated security login failed", "Challenge Length is invalid: " + len.ToString());
                    return false;
                }

                byte[] remoteChallenge;
                if (len == 0)
                {
                    remoteChallenge = new byte[0];
                }
                else
                {
                    remoteChallenge = negotiateStream.ReadBytes(len);
                }

                if (remoteChallenge.SecureEquals(additionalChallenge))
                {
                    return true;
                }
                else
                {
                    Log.Publish(MessageLevel.Info, "Security Login Failed", "Attempting an integrated security login failed", "Challenge did not match. Potential man in the middle attack.");
                    return false;
                }
            }
        }

    }
}
