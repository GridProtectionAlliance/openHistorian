//******************************************************************************************************
//  SecureStreamClientIntegratedSecurity.cs - Gbtc
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

using System.IO;
using GSF.Security.Authentication;

namespace GSF.Security
{
    /// <summary>
    /// Creates a secure stream that connects to a server using windows Integrated Security.
    /// </summary>
    public class SecureStreamClientIntegratedSecurity
        : SecureStreamClientBase
    {
        private readonly IntegratedSecurityClient m_client;

        /// <summary>
        /// Creates a new <see cref="SecureStreamClientIntegratedSecurity"/>
        /// </summary>
        public SecureStreamClientIntegratedSecurity()
        {
            m_client = new IntegratedSecurityClient();
        }

        /// <summary>
        /// Creates a new <see cref="SecureStreamClientIntegratedSecurity"/>
        /// </summary>
        public SecureStreamClientIntegratedSecurity(string username, string password, string domain)
        {
            m_client = new IntegratedSecurityClient(username, password, domain);
        }

        /// <summary>
        /// Authenticates with the remote server.
        /// </summary>
        /// <param name="stream">the stream to authentication on</param>
        /// <param name="certSignatures"></param>
        /// <returns></returns>
        protected override bool InternalTryAuthenticate(Stream stream, byte[] certSignatures)
        {
            stream.WriteByte((byte)AuthenticationMode.Integrated);
            if (m_client.TryAuthenticateAsClient(stream, certSignatures))
            {
                return true;
            }
            return false;
        }
    }
}
