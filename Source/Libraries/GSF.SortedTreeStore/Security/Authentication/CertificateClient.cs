//******************************************************************************************************
//  CertificateClient.cs - Gbtc
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
//  8/29/2014 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

using System;
using System.IO;
using System.Net;
using System.Net.Security;

namespace GSF.Security.Authentication
{
    public class CertificateClient
    {
        private readonly NetworkCredential m_credentials;

        public CertificateClient()
        {
            m_credentials = CredentialCache.DefaultNetworkCredentials;
        }

        public CertificateClient(string username, string password, string domain)
        {
            m_credentials = new NetworkCredential(username, password, domain);
        }

        public bool AuthenticateAsClient(Stream stream)
        {
            using (NegotiateStream negotiateStream = new NegotiateStream(stream, true))
            {
                try
                {
                    negotiateStream.AuthenticateAsClient(m_credentials, string.Empty);
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }

    }
}
