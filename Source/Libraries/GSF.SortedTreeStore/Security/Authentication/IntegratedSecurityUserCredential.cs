//******************************************************************************************************
//  IntegratedSecurityUserCredential.cs - Gbtc
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
using System.Data;
using System.IO;
#if SQLCLR
using System.Security.Principal;
#else
using GSF.Identity;
#endif
using GSF.IO;

namespace GSF.Security.Authentication
{

    /// <summary>
    /// An individual server side user credential
    /// </summary>
    public class IntegratedSecurityUserCredential
    {
        /// <summary>
        /// The username that was passed to the constructor.
        /// </summary>
        public string Username;

        /// <summary>
        /// The security identifier for the username
        /// </summary>
        public string UserID;

        /// <summary>
        /// The token associated with this user and their permissions.
        /// </summary>
        public Guid UserToken;

        /// <summary>
        /// Creates user credentials
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userToken"></param>
        public IntegratedSecurityUserCredential(string username, Guid userToken)
        {
            Username = username;
#if SQLCLR
            SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            UserID = sid.ToString();
#else
            UserID = UserInfo.UserNameToSID(username);
#endif
            UserToken = userToken;
        }

        /// <summary>
        /// Loads user credentials from the supplied stream.
        /// </summary>
        public IntegratedSecurityUserCredential(Stream stream)
        {
            Load(stream);
        }

        /// <summary>
        /// Saves to the supplied stream.
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            stream.WriteByte(1);
            stream.Write(Username);
            stream.Write(UserID);
            stream.Write(UserToken);
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
                    Username = stream.ReadString();
                    UserID = stream.ReadString();
                    UserToken = stream.ReadGuid();
                    return;
                default:
                    throw new VersionNotFoundException("Unknown encoding method");

            }
        }

    }
}


