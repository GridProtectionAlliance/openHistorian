//******************************************************************************************************
//  SecureStream.cs - Gbtc
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

using System.Net.Security;
using System.Text;

namespace GSF.Security
{

    internal enum AuthenticationMode : byte
    {
        None = 1,
        Srp = 2,
        Scram = 3,
        Integrated = 4,
        Certificate = 5,
        ResumeSession = 255
    }

    public class SecureStream
    {
        internal static byte[] ComputeCertificateChallenge(bool isServer, SslStream stream)
        {
            string localChallenge = string.Empty;
            string remoteChallenge = string.Empty;
            if (stream.RemoteCertificate != null)
            {
                remoteChallenge = stream.RemoteCertificate.GetCertHashString();
            }
            if (stream.LocalCertificate != null)
            {
                localChallenge = stream.LocalCertificate.GetCertHashString();
            }
            if (isServer)
            {
                return Encoding.UTF8.GetBytes(remoteChallenge + localChallenge);
            }
            else
            {
                return Encoding.UTF8.GetBytes(localChallenge + remoteChallenge);
            }
        }

    }
}
