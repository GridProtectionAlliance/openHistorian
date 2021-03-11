//******************************************************************************************************
//  CertificateUserCredential.cs - Gbtc
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

#if SQLCLR
using System.Security.Principal;
#else
using GSF.Identity;
#endif

namespace GSF.Security.Authentication
{

    /// <summary>
    /// An individual server side user credential
    /// </summary>
    public class CertificateUserCredential
    {
        public string UserID;

        /// <summary>
        /// Creates user credentials
        /// </summary>
        /// <param name="username"></param>
        public CertificateUserCredential(string username)
        {
#if SQLCLR
            SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            UserID = sid.ToString();
#else
            UserID = UserInfo.UserNameToSID(username);
#endif
        }

        public void Save()
        {

        }

        public void Load()
        {

        }

    }
}


