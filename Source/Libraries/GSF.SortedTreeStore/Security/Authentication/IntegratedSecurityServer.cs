//******************************************************************************************************
//  IntegratedSecurityServer.cs - Gbtc
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
using System.Data;
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
    public class IntegratedSecurityServer
        : DisposableLoggingClassBase
    {
        /// <summary>
        /// The location for all of the supported identities
        /// </summary>
        public IntegratedSecurityUserCredentials Users;

        /// <summary>
        /// Creates a new <see cref="IntegratedSecurityServer"/>.
        /// </summary>
        public IntegratedSecurityServer()
            : base(MessageClass.Component)
        {
            Users = new IntegratedSecurityUserCredentials();
        }

        /// <summary>
        /// Authenticates the client stream
        /// </summary>
        /// <param name="stream">The stream to autenticate</param>
        /// <param name="userToken">the user token associated with the identity match</param>
        /// <param name="additionalChallenge">Additional data that much match between the client and server
        /// for the connection to succeed.</param>
        /// <returns>true if successful authentication. False otherwise.</returns>
        public bool TryAuthenticateAsServer(Stream stream, out Guid userToken, byte[] additionalChallenge = null)
        {
            userToken = Guid.Empty;
            if (additionalChallenge is null)
                additionalChallenge = new byte[] { };
            if (additionalChallenge.Length > short.MaxValue)
                throw new ArgumentOutOfRangeException("additionalChallenge", "Must be less than 32767 bytes");

            using (NegotiateStream negotiateStream = new NegotiateStream(stream, true))
            {
                try
                {
                    negotiateStream.AuthenticateAsServer(CredentialCache.DefaultNetworkCredentials, ProtectionLevel.EncryptAndSign, TokenImpersonationLevel.Identification);
                }
                catch (Exception ex)
                {
                    Log.Publish(MessageLevel.Info, "Security Login Failed", "Attempting an integrated security login failed", null, ex);
                    return false;
                }

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
                    if (Users.TryGetToken(negotiateStream.RemoteIdentity, out userToken))
                    {
                        return true;
                    }
                    Log.Publish(MessageLevel.Info, "Security Login Failed", "Attempting an integrated security login failed", "User did not exist in the database: " + negotiateStream.RemoteIdentity.ToString());
                    return false;
                }

                Log.Publish(MessageLevel.Info, "Security Login Failed", "Attempting an integrated security login failed", "Challenge did not match. Potential man in the middle attack.");
                return false;
            }
        }

        /// <summary>
        /// Saves to the supplied stream.
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            stream.WriteByte(1);
            Users.Save(stream);
        }

        /// <summary>
        /// Loads from the supplied stream.
        /// </summary>
        /// <param name="stream"></param>
        public void Load(Stream stream)
        {
            byte version = stream.ReadNextByte();
            switch (version)
            {
                case 1:
                    Users.Load(stream);
                    return;
                default:
                    throw new VersionNotFoundException("Unknown encoding method");
            }
        }
    }
}
